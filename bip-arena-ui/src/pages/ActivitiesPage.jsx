import { useState, useEffect } from 'react';
import { getUserActivities } from '../services/api';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts';
import UserSelect from '../components/UserSelect';
import Loading from '../components/Loading';

export default function ActivitiesPage() {
    const [userId, setUserId] = useState('U1');
    const [activities, setActivities] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!userId) return;
        setLoading(true);
        getUserActivities(userId)
            .then(setActivities)
            .catch(console.error)
            .finally(() => setLoading(false));
    }, [userId]);

    const chartData = [...activities]
        .reverse()
        .map(a => ({
            date: new Date(a.date).toLocaleDateString('tr-TR', { month: 'short', day: 'numeric' }),
            Mesajlar: a.messages,
            Tepkiler: a.reactions,
            Gruplar: a.uniqueGroups,
        }));

    return (
        <div>
            <UserSelect value={userId} onChange={setUserId} />

            {loading ? (
                <Loading />
            ) : (
                <>
                    {/* Chart */}
                    {chartData.length > 0 && (
                        <div className="card mb-24 animate-in">
                            <div className="card-header">
                                <div className="card-title">📈 Günlük Aktivite Grafiği</div>
                            </div>
                            <div className="chart-container">
                                <ResponsiveContainer width="100%" height="100%">
                                    <BarChart data={chartData}>
                                        <CartesianGrid strokeDasharray="3 3" stroke="rgba(148,163,184,0.1)" />
                                        <XAxis dataKey="date" tick={{ fill: '#94a3b8', fontSize: 12 }} />
                                        <YAxis tick={{ fill: '#94a3b8', fontSize: 12 }} />
                                        <Tooltip
                                            contentStyle={{
                                                background: '#1a2236',
                                                border: '1px solid rgba(148,163,184,0.15)',
                                                borderRadius: 8,
                                                fontSize: 13,
                                            }}
                                        />
                                        <Legend />
                                        <Bar dataKey="Mesajlar" fill="#ffc107" radius={[4, 4, 0, 0]} />
                                        <Bar dataKey="Tepkiler" fill="#00e5ff" radius={[4, 4, 0, 0]} />
                                        <Bar dataKey="Gruplar" fill="#22c55e" radius={[4, 4, 0, 0]} />
                                    </BarChart>
                                </ResponsiveContainer>
                            </div>
                        </div>
                    )}

                    {/* Table */}
                    <div className="card animate-in stagger-1">
                        <div className="card-header">
                            <div className="card-title">📊 Aktivite Geçmişi</div>
                            <span className="pill pill-gold">{activities.length} kayıt</span>
                        </div>
                        <div className="table-wrapper">
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>Tarih</th>
                                        <th>Mesajlar</th>
                                        <th>Tepkiler</th>
                                        <th>Gruplar</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {activities.map(a => (
                                        <tr key={a.eventId}>
                                            <td>{new Date(a.date).toLocaleDateString('tr-TR')}</td>
                                            <td>
                                                <span style={{ fontWeight: 700, color: 'var(--primary)' }}>{a.messages}</span>
                                            </td>
                                            <td>
                                                <span style={{ fontWeight: 700, color: 'var(--accent)' }}>{a.reactions}</span>
                                            </td>
                                            <td>
                                                <span style={{ fontWeight: 700, color: 'var(--success)' }}>{a.uniqueGroups}</span>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
}
