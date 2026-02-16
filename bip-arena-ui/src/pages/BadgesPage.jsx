import { useState, useEffect } from 'react';
import { getAllBadges, getUserBadges } from '../services/api';
import UserSelect from '../components/UserSelect';
import Loading from '../components/Loading';

export default function BadgesPage() {
    const [userId, setUserId] = useState('U1');
    const [allBadges, setAllBadges] = useState([]);
    const [userBadges, setUserBadges] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getAllBadges()
            .then(setAllBadges)
            .catch(console.error)
            .finally(() => setLoading(false));
    }, []);

    useEffect(() => {
        if (!userId) return;
        getUserBadges(userId)
            .then(setUserBadges)
            .catch(console.error);
    }, [userId]);

    if (loading) return <Loading />;

    const earnedBadgeIds = userBadges?.badges?.map(b => b.badgeId) || [];

    return (
        <div>
            <UserSelect value={userId} onChange={setUserId} />

            {/* All Badges */}
            <div className="card mb-24 animate-in">
                <div className="card-header">
                    <div className="card-title">🏅 Tüm Rozetler</div>
                    <span className="pill pill-gold">{allBadges.length} rozet</span>
                </div>

                <div className="badge-grid">
                    {allBadges.map(b => {
                        const earned = earnedBadgeIds.includes(b.badgeId);
                        const tier = b.level === 1 ? 'bronze' : b.level === 2 ? 'silver' : 'gold';
                        const conditionMatch = b.condition.match(/>=\s*(\d+)/);
                        const threshold = conditionMatch ? parseInt(conditionMatch[1]) : 0;

                        return (
                            <div
                                key={b.badgeId}
                                className={`badge-card ${tier} ${earned ? 'earned' : 'locked'}`}
                            >
                                <div className="badge-icon">
                                    {b.level === 1 ? '🥉' : b.level === 2 ? '🥈' : '🥇'}
                                </div>
                                <div className="badge-name">{b.badgeName}</div>
                                <div className="badge-condition" style={{ marginBottom: 8 }}>
                                    {threshold} puan
                                </div>
                                {earned ? (
                                    <span className="pill pill-success">✓ Kazanıldı</span>
                                ) : (
                                    <span className="pill pill-danger">🔒 Kilitli</span>
                                )}
                            </div>
                        );
                    })}
                </div>
            </div>

            {/* User Badge Details */}
            {userBadges && (
                <div className="card animate-in stagger-1">
                    <div className="card-header">
                        <div className="card-title">
                            👤 {userBadges.userName} — Rozet Durumu
                        </div>
                    </div>
                    {userBadges.badges.length === 0 ? (
                        <div className="empty-state">
                            <div className="empty-state-icon">🔒</div>
                            <div className="empty-state-text">Bu kullanıcı henüz rozet kazanmadı</div>
                        </div>
                    ) : (
                        <div className="table-wrapper">
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>Rozet</th>
                                        <th>Ad</th>
                                        <th>Koşul</th>
                                        <th>Seviye</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {userBadges.badges.map(b => (
                                        <tr key={b.badgeId}>
                                            <td style={{ fontSize: 28 }}>
                                                {b.level === 1 ? '🥉' : b.level === 2 ? '🥈' : '🥇'}
                                            </td>
                                            <td style={{ fontWeight: 700, color: 'var(--text-primary)' }}>{b.badgeName}</td>
                                            <td>
                                            </td>
                                            <td>
                                                <span className="pill pill-gold">Seviye {b.level}</span>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
}
