import { useState, useEffect } from 'react';
import { getLeaderboard } from '../services/api';
import Loading from '../components/Loading';

export default function LeaderboardPage() {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getLeaderboard()
            .then(setData)
            .catch(console.error)
            .finally(() => setLoading(false));
    }, []);

    if (loading) return <Loading />;

    const maxPoints = data[0]?.totalPoints || 1;

    return (
        <div>
            <div className="card animate-in">
                <div className="card-header">
                    <div className="card-title">🏆 Sosyal Arena Liderlik Tablosu</div>
                    <span className="pill pill-gold">{data.length} kullanıcı</span>
                </div>

                {data.map((item, idx) => (
                    <div
                        key={item.userId}
                        className={`leaderboard-row ${item.rank <= 3 ? `rank-${item.rank}` : ''}`}
                        style={{ animationDelay: `${idx * 80}ms` }}
                    >
                        <div className={`rank-badge ${item.rank > 3 ? 'default' : ''}`}>
                            {item.rank <= 3 ? ['🥇', '🥈', '🥉'][item.rank - 1] : item.rank}
                        </div>

                        <div className="avatar">{item.name[0]}</div>

                        <div style={{ flex: 1 }}>
                            <div className="leaderboard-name">{item.name}</div>
                            <div style={{ fontSize: 11, color: 'var(--text-muted)' }}>{item.userId}</div>
                            <div className="progress-bar" style={{ marginTop: 6, maxWidth: 300 }}>
                                <div
                                    className="progress-fill"
                                    style={{ width: `${(item.totalPoints / maxPoints) * 100}%` }}
                                />
                            </div>
                        </div>

                        <div style={{ textAlign: 'right' }}>
                            <div className="leaderboard-points">{item.totalPoints}</div>
                            <div style={{ fontSize: 11, color: 'var(--text-muted)' }}>puan</div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}
