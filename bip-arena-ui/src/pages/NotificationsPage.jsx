import { useState, useEffect } from 'react';
import { getUserNotifications, markNotificationRead, markAllNotificationsRead } from '../services/api';
import UserSelect from '../components/UserSelect';
import Loading from '../components/Loading';

export default function NotificationsPage() {
    const [userId, setUserId] = useState('U1');
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(false);

    const loadNotifications = () => {
        if (!userId) return;
        setLoading(true);
        getUserNotifications(userId)
            .then(setData)
            .catch(console.error)
            .finally(() => setLoading(false));
    };

    useEffect(() => {
        loadNotifications();
    }, [userId]);

    const handleMarkRead = async (notificationId) => {
        await markNotificationRead(notificationId);
        loadNotifications();
    };

    const handleMarkAllRead = async () => {
        await markAllNotificationsRead(userId);
        loadNotifications();
    };

    return (
        <div>
            <UserSelect value={userId} onChange={setUserId} />

            {loading ? (
                <Loading />
            ) : data ? (
                <div className="card animate-in">
                    <div className="card-header">
                        <div className="card-title">🔔 Bildirimler</div>
                        <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
                            <span className="pill pill-info">{data.unreadCount} okunmadı</span>
                            <span className="pill pill-gold">{data.totalCount} toplam</span>
                            {data.unreadCount > 0 && (
                                <button className="btn btn-sm btn-secondary" onClick={handleMarkAllRead}>
                                    ✓ Tümünü Okundu Yap
                                </button>
                            )}
                        </div>
                    </div>

                    {data.notifications.length === 0 ? (
                        <div className="empty-state">
                            <div className="empty-state-icon">🔔</div>
                            <div className="empty-state-text">Bildirim yok</div>
                        </div>
                    ) : (
                        data.notifications.map(n => (
                            <div
                                key={n.notificationId}
                                className={`notification-item ${!n.isRead ? 'unread' : ''}`}
                            >
                                <div className={`notification-icon ${n.type === 'CHALLENGE' ? 'challenge' : 'badge'}`}>
                                    {n.type === 'CHALLENGE' ? '🎯' : '🏅'}
                                </div>
                                <div className="notification-body">
                                    <div className="notification-message">{n.message}</div>
                                    <div className="notification-meta">
                                        {n.type} · {n.sourceRef}
                                    </div>
                                </div>
                                {!n.isRead && (
                                    <button
                                        className="btn btn-sm btn-ghost"
                                        onClick={() => handleMarkRead(n.notificationId)}
                                    >
                                        Okundu ✓
                                    </button>
                                )}
                            </div>
                        ))
                    )}
                </div>
            ) : null}
        </div>
    );
}
