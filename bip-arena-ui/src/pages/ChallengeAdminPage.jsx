import { useState, useEffect } from 'react';
import { getChallenges } from '../services/api';
import Loading from '../components/Loading';

export default function ChallengeAdminPage() {
    const [challenges, setChallenges] = useState([]);
    const [loading, setLoading] = useState(true);
    const [editingId, setEditingId] = useState(null);
    const [editValues, setEditValues] = useState({});

    useEffect(() => {
        getChallenges()
            .then(setChallenges)
            .catch(console.error)
            .finally(() => setLoading(false));
    }, []);

    const startEdit = (c) => {
        setEditingId(c.challengeId);
        setEditValues({ ...c });
    };

    const cancelEdit = () => {
        setEditingId(null);
        setEditValues({});
    };

    const saveEdit = () => {
        setChallenges(prev =>
            prev.map(c => c.challengeId === editingId ? { ...c, ...editValues } : c)
        );
        setEditingId(null);
        setEditValues({});
    };

    const toggleActive = (challengeId) => {
        setChallenges(prev =>
            prev.map(c => c.challengeId === challengeId ? { ...c, isActive: !c.isActive } : c)
        );
    };

    if (loading) return <Loading />;

    return (
        <div>
            <div className="card mb-24 animate-in">
                <div className="card-header">
                    <div className="card-title">⚙️ Challenge Yönetim Paneli</div>
                    <span className="pill pill-info">Yerel düzenleme (mock)</span>
                </div>
                <p style={{ fontSize: 13, color: 'var(--text-secondary)', marginBottom: 20 }}>
                    Challenge'ları düzenleyebilir, aktif/pasif yapabilirsiniz. Değişiklikler frontend'de uygulanır.
                </p>

                <div className="table-wrapper">
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Ad</th>
                                <th>Koşul</th>
                                <th>Puan</th>
                                <th>Öncelik</th>
                                <th>Durum</th>
                                <th>Aksiyonlar</th>
                            </tr>
                        </thead>
                        <tbody>
                            {challenges.map(c => (
                                <tr key={c.challengeId}>
                                    <td style={{ fontFamily: 'monospace', fontSize: 12 }}>{c.challengeId}</td>

                                    {editingId === c.challengeId ? (
                                        <>
                                            <td>
                                                <input
                                                    className="form-input"
                                                    value={editValues.challengeName}
                                                    onChange={(e) => setEditValues({ ...editValues, challengeName: e.target.value })}
                                                    style={{ minWidth: 140 }}
                                                />
                                            </td>
                                            <td>
                                                <input
                                                    className="form-input"
                                                    value={editValues.condition}
                                                    onChange={(e) => setEditValues({ ...editValues, condition: e.target.value })}
                                                    style={{ minWidth: 160, fontFamily: 'monospace', fontSize: 12 }}
                                                />
                                            </td>
                                            <td>
                                                <input
                                                    className="form-input"
                                                    type="number"
                                                    value={editValues.rewardPoints}
                                                    onChange={(e) => setEditValues({ ...editValues, rewardPoints: parseInt(e.target.value) || 0 })}
                                                    style={{ width: 80 }}
                                                />
                                            </td>
                                            <td>
                                                <input
                                                    className="form-input"
                                                    type="number"
                                                    value={editValues.priority}
                                                    onChange={(e) => setEditValues({ ...editValues, priority: parseInt(e.target.value) || 0 })}
                                                    style={{ width: 60 }}
                                                />
                                            </td>
                                        </>
                                    ) : (
                                        <>
                                            <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{c.challengeName}</td>
                                            <td>
                                                <span className="challenge-condition">{c.condition}</span>
                                            </td>
                                            <td>
                                                <span style={{ fontWeight: 700, color: 'var(--primary)' }}>+{c.rewardPoints}</span>
                                            </td>
                                            <td>
                                                <span className="pill pill-gold">P{c.priority}</span>
                                            </td>
                                        </>
                                    )}

                                    <td>
                                        <button
                                            className={`btn btn-sm ${c.isActive ? 'btn-primary' : 'btn-danger'}`}
                                            onClick={() => toggleActive(c.challengeId)}
                                            style={{ fontSize: 11, minWidth: 60 }}
                                        >
                                            {c.isActive ? 'Aktif' : 'Pasif'}
                                        </button>
                                    </td>

                                    <td>
                                        {editingId === c.challengeId ? (
                                            <div style={{ display: 'flex', gap: 4 }}>
                                                <button className="btn btn-sm btn-primary" onClick={saveEdit}>💾</button>
                                                <button className="btn btn-sm btn-ghost" onClick={cancelEdit}>✕</button>
                                            </div>
                                        ) : (
                                            <button className="btn btn-sm btn-secondary" onClick={() => startEdit(c)}>✏️ Düzenle</button>
                                        )}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {/* Priority Explanation */}
            <div className="card animate-in stagger-1">
                <div className="card-header">
                    <div className="card-title">📌 Çakışma Kuralları</div>
                </div>
                <div style={{ fontSize: 13, color: 'var(--text-secondary)', lineHeight: 1.8 }}>
                    <p>Bir kullanıcı aynı gün birden fazla challenge'ı tamamlarsa:</p>
                    <ol style={{ paddingLeft: 20, marginTop: 8 }}>
                        <li><strong>Priority değeri en küçük</strong> olan challenge seçilir (en öncelikli)</li>
                        <li>Diğer tetiklenen challenge'lar <span className="pill pill-danger">suppressed</span> olarak kaydedilir</li>
                        <li>Sadece seçilen challenge'ın puanı ledger'a yazılır</li>
                    </ol>
                    <div style={{ marginTop: 16 }}>
                        <strong>Öncelik sırası:</strong>
                        {challenges.sort((a, b) => a.priority - b.priority).map(c => (
                            <span key={c.challengeId} className="pill pill-gold" style={{ marginLeft: 8 }}>
                                P{c.priority} — {c.challengeName}
                            </span>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
}
