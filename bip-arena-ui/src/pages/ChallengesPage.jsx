import { useState, useEffect } from 'react';
import { getChallenges, evaluateChallenges, calculateMetrics, evaluateBadges } from '../services/api';
import Loading from '../components/Loading';

const AS_OF_DATE = '2026-03-12';

export default function ChallengesPage() {
    const [challenges, setChallenges] = useState([]);
    const [results, setResults] = useState(null);
    const [loading, setLoading] = useState(true);
    const [evaluating, setEvaluating] = useState(false);
    const [step, setStep] = useState('');

    useEffect(() => {
        getChallenges()
            .then(setChallenges)
            .catch(console.error)
            .finally(() => setLoading(false));
    }, []);

    const handleRunPipeline = async () => {
        setEvaluating(true);
        try {
            setStep('Metrikler hesaplanıyor...');
            await calculateMetrics(AS_OF_DATE);

            setStep('Challenge\'lar değerlendiriliyor...');
            const evalResults = await evaluateChallenges(AS_OF_DATE);
            setResults(evalResults);

            setStep('Rozetler değerlendiriliyor...');
            await evaluateBadges();

            setStep('Tamamlandı! ✅');
        } catch (e) {
            console.error(e);
            setStep('Hata oluştu ❌');
        }
        setEvaluating(false);
    };

    if (loading) return <Loading />;

    return (
        <div>
            {/* Challenge Definitions */}
            <div className="card mb-24 animate-in">
                <div className="card-header">
                    <div className="card-title">🎯 Tanımlı Challenge'lar</div>
                    <span className="pill pill-gold">{challenges.length} aktif</span>
                </div>
                <div className="challenge-grid">
                    {challenges.map(c => (
                        <div key={c.challengeId} className="challenge-card">
                            <div className="challenge-name">{c.challengeName}</div>
                            <div className="challenge-condition">{c.condition}</div>
                            <div className="challenge-reward">⭐ +{c.rewardPoints} puan</div>
                            <div className="challenge-priority">Öncelik: {c.priority}</div>
                        </div>
                    ))}
                </div>
            </div>

            {/* Run Engine */}
            <div className="card mb-24 animate-in stagger-1">
                <div className="card-header">
                    <div className="card-title">⚡ Challenge Motoru</div>
                </div>
                <p style={{ fontSize: 13, color: 'var(--text-secondary)', marginBottom: 16 }}>
                    Bu işlem sırasıyla metrikleri hesaplar → challenge'ları değerlendirir → rozetleri kontrol eder.
                    <br />
                    <strong>AsOfDate:</strong> {AS_OF_DATE}
                </p>
                <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
                    <button
                        className="btn btn-primary"
                        onClick={handleRunPipeline}
                        disabled={evaluating}
                    >
                        {evaluating ? '⏳ Çalışıyor...' : '🚀 Pipeline Çalıştır'}
                    </button>
                    {step && (
                        <span style={{ fontSize: 13, color: 'var(--accent)' }}>{step}</span>
                    )}
                </div>
            </div>

            {/* Results */}
            {results && (
                <div className="card animate-in stagger-2">
                    <div className="card-header">
                        <div className="card-title">📊 Değerlendirme Sonuçları</div>
                    </div>
                    <div className="table-wrapper">
                        <table className="data-table">
                            <thead>
                                <tr>
                                    <th>Kullanıcı</th>
                                    <th>Tetiklenen</th>
                                    <th>Seçilen</th>
                                    <th>Bastırılan</th>
                                    <th>Puan</th>
                                </tr>
                            </thead>
                            <tbody>
                                {results.map(r => (
                                    <tr key={r.userId}>
                                        <td>
                                            <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                                                <div className="avatar avatar-sm">{r.userName[0]}</div>
                                                <div>
                                                    <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{r.userName}</div>
                                                    <div style={{ fontSize: 11, color: 'var(--text-muted)' }}>{r.userId}</div>
                                                </div>
                                            </div>
                                        </td>
                                        <td>
                                            {r.triggeredChallenges.length === 0 ? (
                                                <span style={{ color: 'var(--text-muted)' }}>-</span>
                                            ) : (
                                                r.triggeredChallenges.map(c => (
                                                    <span key={c} className="pill pill-warning" style={{ marginRight: 4, marginBottom: 4 }}>{c}</span>
                                                ))
                                            )}
                                        </td>
                                        <td>
                                            {r.selectedChallenge ? (
                                                <span className="pill pill-success">{r.selectedChallenge}</span>
                                            ) : '-'}
                                        </td>
                                        <td>
                                            {r.suppressedChallenges?.length > 0 ? (
                                                r.suppressedChallenges.map(c => (
                                                    <span key={c} className="pill pill-danger" style={{ marginRight: 4 }}>{c}</span>
                                                ))
                                            ) : '-'}
                                        </td>
                                        <td>
                                            {r.pointsAwarded > 0 ? (
                                                <span style={{ fontWeight: 800, color: 'var(--primary)', fontSize: 16 }}>+{r.pointsAwarded}</span>
                                            ) : (
                                                <span style={{ color: 'var(--text-muted)' }}>0</span>
                                            )}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            )}
        </div>
    );
}
