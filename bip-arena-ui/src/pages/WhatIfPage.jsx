import { useState, useEffect } from 'react';
import { getUserMetrics, getChallenges, getAllBadges } from '../services/api';
import UserSelect from '../components/UserSelect';
import Loading from '../components/Loading';

const AS_OF_DATE = '2026-03-12';

function evaluateCondition(condition, state) {
    const parts = condition.split(/>=|<=|>|<|==/);
    if (parts.length !== 2) return false;
    const metric = parts[0].trim();
    const threshold = parseInt(parts[1].trim());
    if (isNaN(threshold)) return false;

    const metricMap = {
        messages_today: state.messagesToday,
        reactions_today: state.reactionsToday,
        unique_groups_today: state.uniqueGroupsToday,
        messages_7d: state.messages7d,
        reactions_7d: state.reactions7d,
    };

    const value = metricMap[metric] ?? 0;

    if (condition.includes('>=')) return value >= threshold;
    if (condition.includes('<=')) return value <= threshold;
    if (condition.includes('>')) return value > threshold;
    if (condition.includes('<')) return value < threshold;
    if (condition.includes('==')) return value === threshold;
    return false;
}

export default function WhatIfPage() {
    const [userId, setUserId] = useState('U1');
    const [metrics, setMetrics] = useState(null);
    const [challenges, setChallenges] = useState([]);
    const [badges, setBadges] = useState([]);
    const [loading, setLoading] = useState(false);

    // What-if deltas
    const [deltaMessages, setDeltaMessages] = useState(0);
    const [deltaReactions, setDeltaReactions] = useState(0);
    const [deltaGroups, setDeltaGroups] = useState(0);

    useEffect(() => {
        if (!userId) return;
        setLoading(true);
        Promise.all([
            getUserMetrics(userId, AS_OF_DATE),
            getChallenges(),
            getAllBadges(),
        ])
            .then(([m, c, b]) => {
                setMetrics(m);
                setChallenges(c);
                setBadges(b);
                setDeltaMessages(0);
                setDeltaReactions(0);
                setDeltaGroups(0);
            })
            .catch(console.error)
            .finally(() => setLoading(false));
    }, [userId]);

    if (loading) return <Loading />;
    if (!metrics) {
        return (
            <div>
                <UserSelect value={userId} onChange={setUserId} />
                <div className="empty-state">
                    <div className="empty-state-icon">🔮</div>
                    <div className="empty-state-text">Kullanıcı seçin ve metrikleri yükleyin</div>
                </div>
            </div>
        );
    }

    // Simulated state
    const simState = {
        messagesToday: metrics.messagesToday + deltaMessages,
        reactionsToday: metrics.reactionsToday + deltaReactions,
        uniqueGroupsToday: metrics.uniqueGroupsToday + deltaGroups,
        messages7d: metrics.messages7d + deltaMessages,
        reactions7d: metrics.reactions7d + deltaReactions,
    };

    // Original evaluation
    const origTriggered = challenges.filter(c => evaluateCondition(c.condition, metrics));
    const origSelected = origTriggered.length > 0
        ? origTriggered.reduce((best, c) => c.priority < best.priority ? c : best)
        : null;

    // Simulated evaluation
    const simTriggered = challenges.filter(c => evaluateCondition(c.condition, simState));
    const simSelected = simTriggered.length > 0
        ? simTriggered.reduce((best, c) => c.priority < best.priority ? c : best)
        : null;

    const pointDiff = (simSelected?.rewardPoints || 0) - (origSelected?.rewardPoints || 0);
    const newChallenges = simTriggered.filter(c => !origTriggered.find(o => o.challengeId === c.challengeId));

    return (
        <div>
            <UserSelect value={userId} onChange={setUserId} />

            {/* What-If Input Panel */}
            <div className="whatif-panel animate-in">
                <div className="card-title mb-16">🔮 What-If Simülasyonu</div>
                <p style={{ fontSize: 13, color: 'var(--text-secondary)', marginBottom: 16 }}>
                    Ek aktivite ekleyerek hangi challenge'ların tetikleneceğini simüle edin.
                </p>

                <div className="whatif-inputs">
                    <div className="form-group">
                        <label className="form-label">+Mesaj Sayısı</label>
                        <input
                            className="form-input"
                            type="number"
                            min="0"
                            value={deltaMessages}
                            onChange={(e) => setDeltaMessages(parseInt(e.target.value) || 0)}
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label">+Tepki Sayısı</label>
                        <input
                            className="form-input"
                            type="number"
                            min="0"
                            value={deltaReactions}
                            onChange={(e) => setDeltaReactions(parseInt(e.target.value) || 0)}
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label">+Grup Sayısı</label>
                        <input
                            className="form-input"
                            type="number"
                            min="0"
                            value={deltaGroups}
                            onChange={(e) => setDeltaGroups(parseInt(e.target.value) || 0)}
                        />
                    </div>
                </div>
            </div>

            {/* Comparison */}
            <div className="grid-2 mb-24">
                {/* Original */}
                <div className="card animate-in stagger-1">
                    <div className="card-header">
                        <div className="card-title">📊 Mevcut Durum</div>
                    </div>
                    <div style={{ display: 'grid', gap: 8, marginBottom: 16 }}>
                        <div className="flex-between" style={{ fontSize: 13 }}>
                            <span style={{ color: 'var(--text-muted)' }}>Mesajlar (bugün)</span>
                            <span style={{ fontWeight: 700 }}>{metrics.messagesToday}</span>
                        </div>
                        <div className="flex-between" style={{ fontSize: 13 }}>
                            <span style={{ color: 'var(--text-muted)' }}>Tepkiler (bugün)</span>
                            <span style={{ fontWeight: 700 }}>{metrics.reactionsToday}</span>
                        </div>
                        <div className="flex-between" style={{ fontSize: 13 }}>
                            <span style={{ color: 'var(--text-muted)' }}>Gruplar (bugün)</span>
                            <span style={{ fontWeight: 700 }}>{metrics.uniqueGroupsToday}</span>
                        </div>
                        <div className="flex-between" style={{ fontSize: 13 }}>
                            <span style={{ color: 'var(--text-muted)' }}>Mesajlar (7g)</span>
                            <span style={{ fontWeight: 700 }}>{metrics.messages7d}</span>
                        </div>
                        <div className="flex-between" style={{ fontSize: 13 }}>
                            <span style={{ color: 'var(--text-muted)' }}>Tepkiler (7g)</span>
                            <span style={{ fontWeight: 700 }}>{metrics.reactions7d}</span>
                        </div>
                    </div>

                    <div style={{ borderTop: '1px solid var(--border)', paddingTop: 12 }}>
                        <div style={{ fontSize: 12, color: 'var(--text-muted)', marginBottom: 8 }}>Tetiklenen:</div>
                        {origTriggered.length === 0 ? (
                            <span style={{ fontSize: 12, color: 'var(--text-muted)' }}>Yok</span>
                        ) : (
                            origTriggered.map(c => (
                                <span key={c.challengeId} className={`pill ${c.challengeId === origSelected?.challengeId ? 'pill-success' : 'pill-warning'}`} style={{ marginRight: 4, marginBottom: 4 }}>
                                    {c.challengeName} {c.challengeId === origSelected?.challengeId ? '✓' : ''}
                                </span>
                            ))
                        )}
                        {origSelected && (
                            <div style={{ fontSize: 14, fontWeight: 800, color: 'var(--primary)', marginTop: 8 }}>
                                +{origSelected.rewardPoints} puan
                            </div>
                        )}
                    </div>
                </div>

                {/* Simulated */}
                <div className="card animate-in stagger-2" style={{
                    border: '1px solid rgba(0,229,255,0.2)',
                    background: 'linear-gradient(135deg, rgba(0,229,255,0.05), var(--bg-card))'
                }}>
                    <div className="card-header">
                        <div className="card-title" style={{ color: 'var(--accent)' }}>🔮 Simülasyon Sonucu</div>
                    </div>
                    <div style={{ display: 'grid', gap: 8, marginBottom: 16 }}>
                        <div className="flex-between" style={{ fontSize: 13 }}>
                            <span style={{ color: 'var(--text-muted)' }}>Mesajlar (bugün)</span>
                            <span style={{ fontWeight: 700, color: deltaMessages > 0 ? 'var(--accent)' : '' }}>
                                {simState.messagesToday} {deltaMessages > 0 && `(+${deltaMessages})`}
                            </span>
                        </div>
                        <div className="flex-between" style={{ fontSize: 13 }}>
                            <span style={{ color: 'var(--text-muted)' }}>Tepkiler (bugün)</span>
                            <span style={{ fontWeight: 700, color: deltaReactions > 0 ? 'var(--accent)' : '' }}>
                                {simState.reactionsToday} {deltaReactions > 0 && `(+${deltaReactions})`}
                            </span>
                        </div>
                        <div className="flex-between" style={{ fontSize: 13 }}>
                            <span style={{ color: 'var(--text-muted)' }}>Gruplar (bugün)</span>
                            <span style={{ fontWeight: 700, color: deltaGroups > 0 ? 'var(--accent)' : '' }}>
                                {simState.uniqueGroupsToday} {deltaGroups > 0 && `(+${deltaGroups})`}
                            </span>
                        </div>
                        <div className="flex-between" style={{ fontSize: 13 }}>
                            <span style={{ color: 'var(--text-muted)' }}>Mesajlar (7g)</span>
                            <span style={{ fontWeight: 700, color: deltaMessages > 0 ? 'var(--accent)' : '' }}>
                                {simState.messages7d}
                            </span>
                        </div>
                        <div className="flex-between" style={{ fontSize: 13 }}>
                            <span style={{ color: 'var(--text-muted)' }}>Tepkiler (7g)</span>
                            <span style={{ fontWeight: 700, color: deltaReactions > 0 ? 'var(--accent)' : '' }}>
                                {simState.reactions7d}
                            </span>
                        </div>
                    </div>

                    <div style={{ borderTop: '1px solid var(--border)', paddingTop: 12 }}>
                        <div style={{ fontSize: 12, color: 'var(--text-muted)', marginBottom: 8 }}>Tetiklenen:</div>
                        {simTriggered.length === 0 ? (
                            <span style={{ fontSize: 12, color: 'var(--text-muted)' }}>Yok</span>
                        ) : (
                            simTriggered.map(c => (
                                <span key={c.challengeId} className={`pill ${c.challengeId === simSelected?.challengeId ? 'pill-success' : 'pill-warning'}`} style={{ marginRight: 4, marginBottom: 4 }}>
                                    {c.challengeName} {c.challengeId === simSelected?.challengeId ? '✓' : ''}
                                    {newChallenges.find(n => n.challengeId === c.challengeId) ? ' 🆕' : ''}
                                </span>
                            ))
                        )}
                        {simSelected && (
                            <div style={{ fontSize: 14, fontWeight: 800, color: 'var(--accent)', marginTop: 8 }}>
                                +{simSelected.rewardPoints} puan
                                {pointDiff !== 0 && (
                                    <span style={{
                                        marginLeft: 8,
                                        fontSize: 12,
                                        color: pointDiff > 0 ? 'var(--success)' : 'var(--danger)'
                                    }}>
                                        ({pointDiff > 0 ? '+' : ''}{pointDiff} fark)
                                    </span>
                                )}
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {/* Challenge-by-challenge breakdown */}
            <div className="card animate-in stagger-3">
                <div className="card-header">
                    <div className="card-title">🎯 Challenge Bazlı Kıyaslama</div>
                </div>
                <div className="table-wrapper">
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Challenge</th>
                                <th>Koşul</th>
                                <th>Mevcut Değer</th>
                                <th>Simüle Değer</th>
                                <th>Mevcut</th>
                                <th>Simüle</th>
                            </tr>
                        </thead>
                        <tbody>
                            {challenges.map(c => {
                                const origPasses = evaluateCondition(c.condition, metrics);
                                const simPasses = evaluateCondition(c.condition, simState);

                                // Extract metric and threshold
                                const parts = c.condition.split(/>=|<=|>|<|==/);
                                const metric = parts[0]?.trim() || '';
                                const threshold = parseInt(parts[1]?.trim()) || 0;
                                const metricMapOrig = {
                                    messages_today: metrics.messagesToday,
                                    reactions_today: metrics.reactionsToday,
                                    unique_groups_today: metrics.uniqueGroupsToday,
                                    messages_7d: metrics.messages7d,
                                    reactions_7d: metrics.reactions7d,
                                };
                                const metricMapSim = {
                                    messages_today: simState.messagesToday,
                                    reactions_today: simState.reactionsToday,
                                    unique_groups_today: simState.uniqueGroupsToday,
                                    messages_7d: simState.messages7d,
                                    reactions_7d: simState.reactions7d,
                                };
                                const origVal = metricMapOrig[metric] ?? '-';
                                const simVal = metricMapSim[metric] ?? '-';

                                return (
                                    <tr key={c.challengeId}>
                                        <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{c.challengeName}</td>
                                        <td>
                                            <span className="challenge-condition">{c.condition}</span>
                                        </td>
                                        <td style={{ fontWeight: 700 }}>
                                            {origVal} / {threshold}
                                        </td>
                                        <td style={{ fontWeight: 700, color: simVal !== origVal ? 'var(--accent)' : '' }}>
                                            {simVal} / {threshold}
                                        </td>
                                        <td>
                                            {origPasses ? (
                                                <span className="pill pill-success">✓</span>
                                            ) : (
                                                <span className="pill pill-danger">✗</span>
                                            )}
                                        </td>
                                        <td>
                                            {simPasses ? (
                                                <span className="pill pill-success">✓</span>
                                            ) : (
                                                <span className="pill pill-danger">✗</span>
                                            )}
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}
