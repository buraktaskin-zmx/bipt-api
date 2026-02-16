import { useState, useEffect } from 'react';
import { getUserDashboard, getLeaderboard, getUserMetrics, getUserChallengeAwards, getUserNotifications } from '../services/api';
import UserSelect from '../components/UserSelect';
import Loading from '../components/Loading';

const AS_OF_DATE = '2026-03-12';

export default function DashboardPage() {
    const [userId, setUserId] = useState('U1');
    const [dashboard, setDashboard] = useState(null);
    const [metrics, setMetrics] = useState(null);
    const [awards, setAwards] = useState([]);
    const [notifications, setNotifications] = useState(null);
    const [leaderboard, setLeaderboard] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!userId) return;
        setLoading(true);
        Promise.all([
            getUserDashboard(userId),
            getUserMetrics(userId, AS_OF_DATE),
            getUserChallengeAwards(userId, AS_OF_DATE),
            getUserNotifications(userId),
            getLeaderboard(),
        ])
            .then(([dash, met, aw, notif, lb]) => {
                setDashboard(dash);
                setMetrics(met);
                setAwards(aw);
                setNotifications(notif);
                setLeaderboard(lb);
            })
            .catch(console.error)
            .finally(() => setLoading(false));
    }, [userId]);

    if (loading) return <Loading text="Dashboard yükleniyor..." />;

    return (
        <div>
            <UserSelect value={userId} onChange={setUserId} />

            {dashboard && (
                <>
                    {/* User Overview */}
                    <div className="card mb-24 animate-in" style={{
                        background: 'linear-gradient(135deg, rgba(255,193,7,0.08), rgba(0,229,255,0.05))',
                        border: '1px solid rgba(255,193,7,0.15)'
                    }}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 20 }}>
                            <div className="avatar avatar-lg">{dashboard.user.name[0]}</div>
                            <div style={{ flex: 1 }}>
                                <h2 style={{ fontSize: 22, fontWeight: 800 }}>{dashboard.user.name}</h2>
                                <span style={{ fontSize: 13, color: 'var(--text-muted)' }}>
                                    📍 {dashboard.user.city} · {dashboard.user.userId}
                                </span>
                            </div>
                            <div style={{ textAlign: 'right' }}>
                                <div style={{ fontSize: 32, fontWeight: 900, color: 'var(--primary)' }}>
                                    #{dashboard.leaderboardRank}
                                </div>
                                <div style={{ fontSize: 12, color: 'var(--text-muted)' }}>Sıralama</div>
                            </div>
                        </div>
                    </div>

                    {/* Stats */}
                    <div className="stats-grid animate-in stagger-1">
                        <div className="stat-card">
                            <div className="stat-icon">⭐</div>
                            <div className="stat-label">Toplam Puan</div>
                            <div className="stat-value gold">{dashboard.totalPoints}</div>
                        </div>
                        {metrics && (
                            <>
                                <div className="stat-card">
                                    <div className="stat-icon">💬</div>
                                    <div className="stat-label">Bugün Mesaj</div>
                                    <div className="stat-value">{metrics.messagesToday}</div>
                                    <div className="stat-sub">7 gün: {metrics.messages7d}</div>
                                </div>
                                <div className="stat-card">
                                    <div className="stat-icon">💜</div>
                                    <div className="stat-label">Bugün Tepki</div>
                                    <div className="stat-value accent">{metrics.reactionsToday}</div>
                                    <div className="stat-sub">7 gün: {metrics.reactions7d}</div>
                                </div>
                                <div className="stat-card">
                                    <div className="stat-icon">👥</div>
                                    <div className="stat-label">Aktif Grup</div>
                                    <div className="stat-value success">{metrics.uniqueGroupsToday}</div>
                                    <div className="stat-sub">Bugün</div>
                                </div>
                            </>
                        )}
                    </div>

                    {/* Challenge Awards & Badges */}
                    <div className="grid-2 mb-24">
                        {/* Challenge Awards */}
                        <div className="card animate-in stagger-2">
                            <div className="card-header">
                                <div className="card-title">🎯 Challenge Sonuçları</div>
                                <span className="pill pill-gold">{AS_OF_DATE}</span>
                            </div>
                            {awards.length === 0 ? (
                                <div className="empty-state">
                                    <div className="empty-state-icon">🎯</div>
                                    <div className="empty-state-text">Henüz challenge değerlendirmesi yapılmadı</div>
                                </div>
                            ) : (
                                awards.map(a => (
                                    <div
                                        key={a.awardId}
                                        className={`challenge-card ${a.isSelected ? 'selected' : 'suppressed'}`}
                                        style={{ marginBottom: 8 }}
                                    >
                                        {a.isSelected && <div className="challenge-status-tag selected">✓ Seçildi</div>}
                                        {!a.isSelected && <div className="challenge-status-tag suppressed">Bastırıldı</div>}
                                        <div className="challenge-name">{a.challengeName}</div>
                                        <div className="challenge-reward">
                                            ⭐ +{a.rewardPoints} puan
                                        </div>
                                    </div>
                                ))
                            )}
                        </div>

                        {/* Badges */}
                        <div className="card animate-in stagger-3">
                            <div className="card-header">
                                <div className="card-title">🏅 Rozetler</div>
                            </div>
                            {dashboard.badges.length === 0 ? (
                                <div className="empty-state">
                                    <div className="empty-state-icon">🏅</div>
                                    <div className="empty-state-text">Rozet kazanılmadı</div>
                                </div>
                            ) : (
                                <div className="badge-grid">
                                    {dashboard.badges.map(b => {
                                        const tier = b.level === 1 ? 'bronze' : b.level === 2 ? 'silver' : 'gold';
                                        return (
                                            <div key={b.badgeId} className={`badge-card ${tier} earned`}>
                                                <div className="badge-icon">
                                                    {b.level === 1 ? '🥉' : b.level === 2 ? '🥈' : '🥇'}
                                                </div>
                                                <div className="badge-name">{b.badgeName}</div>
                                                <div className="badge-condition">{b.condition}</div>
                                            </div>
                                        );
                                    })}
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Notifications & Mini Leaderboard */}
                    <div className="grid-2">
                        {/* Notifications */}
                        <div className="card animate-in stagger-4">
                            <div className="card-header">
                                <div className="card-title">🔔 Son Bildirimler</div>
                                {notifications && (
                                    <span className="pill pill-info">{notifications.unreadCount} okunmadı</span>
                                )}
                            </div>
                            {notifications?.notifications?.length === 0 ? (
                                <div className="empty-state">
                                    <div className="empty-state-icon">🔔</div>
                                    <div className="empty-state-text">Bildirim yok</div>
                                </div>
                            ) : (
                                notifications?.notifications?.slice(0, 5).map(n => (
                                    <div key={n.notificationId} className={`notification-item ${!n.isRead ? 'unread' : ''}`}>
                                        <div className={`notification-icon ${n.type === 'CHALLENGE' ? 'challenge' : 'badge'}`}>
                                            {n.type === 'CHALLENGE' ? '🎯' : '🏅'}
                                        </div>
                                        <div className="notification-body">
                                            <div className="notification-message">{n.message}</div>
                                            <div className="notification-meta">{n.type}</div>
                                        </div>
                                    </div>
                                ))
                            )}
                        </div>

                        {/* Mini Leaderboard */}
                        <div className="card animate-in stagger-5">
                            <div className="card-header">
                                <div className="card-title">🏆 Leaderboard</div>
                            </div>
                            {leaderboard.slice(0, 5).map(item => (
                                <div
                                    key={item.userId}
                                    className={`leaderboard-row ${item.rank <= 3 ? `rank-${item.rank}` : ''} ${item.userId === userId ? 'rank-1' : ''}`}
                                    style={item.userId === userId ? { outline: '2px solid var(--primary)', outlineOffset: -1 } : {}}
                                >
                                    <div className={`rank-badge ${item.rank > 3 ? 'default' : ''}`}>
                                        {item.rank <= 3 ? ['🥇', '🥈', '🥉'][item.rank - 1] : item.rank}
                                    </div>
                                    <div className="leaderboard-name">
                                        {item.name}
                                        <div style={{ fontSize: 11, color: 'var(--text-muted)' }}>{item.userId}</div>
                                    </div>
                                    <div className="leaderboard-points">{item.totalPoints} pt</div>
                                </div>
                            ))}
                        </div>
                    </div>

                    {/* Recent Activity */}
                    {dashboard.recentActivity?.length > 0 && (
                        <div className="card mt-16 animate-in">
                            <div className="card-header">
                                <div className="card-title">📒 Son Puan Hareketleri</div>
                            </div>
                            <div className="table-wrapper">
                                <table className="data-table">
                                    <thead>
                                        <tr>
                                            <th>Ledger ID</th>
                                            <th>Puan</th>
                                            <th>Kaynak</th>
                                            <th>Referans</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {dashboard.recentActivity.map(a => (
                                            <tr key={a.ledgerId}>
                                                <td style={{ fontFamily: 'monospace', fontSize: 12 }}>{a.ledgerId}</td>
                                                <td>
                                                    <span className="pill pill-success">+{a.pointsDelta}</span>
                                                </td>
                                                <td>{a.source}</td>
                                                <td style={{ fontFamily: 'monospace', fontSize: 12 }}>{a.sourceRef}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    )}
                </>
            )}

            {!userId && (
                <div className="empty-state">
                    <div className="empty-state-icon">👆</div>
                    <div className="empty-state-text">Başlamak için bir kullanıcı seçin</div>
                </div>
            )}
        </div>
    );
}
