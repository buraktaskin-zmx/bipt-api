import { useState, useEffect } from 'react';
import { getUserLedger, getLedgerStatistics } from '../services/api';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import UserSelect from '../components/UserSelect';
import Loading from '../components/Loading';

export default function LedgerPage() {
    const [userId, setUserId] = useState('U1');
    const [ledgerData, setLedgerData] = useState(null);
    const [stats, setStats] = useState([]);
    const [loading, setLoading] = useState(false);
    const [tab, setTab] = useState('user');

    useEffect(() => {
        if (tab === 'stats') {
            setLoading(true);
            getLedgerStatistics()
                .then(setStats)
                .catch(console.error)
                .finally(() => setLoading(false));
        }
    }, [tab]);

    useEffect(() => {
        if (!userId || tab !== 'user') return;
        setLoading(true);
        getUserLedger(userId)
            .then(setLedgerData)
            .catch(console.error)
            .finally(() => setLoading(false));
    }, [userId, tab]);

    // Cumulative points chart data
    const chartData = ledgerData?.transactions
        ? [...ledgerData.transactions].reverse().reduce((acc, t, i) => {
            const prev = i > 0 ? acc[i - 1].cumulative : 0;
            acc.push({ name: t.ledgerId, puan: t.pointsDelta, cumulative: prev + t.pointsDelta });
            return acc;
        }, [])
        : [];

    return (
        <div>
            <div className="tabs">
                <button className={`tab ${tab === 'user' ? 'active' : ''}`} onClick={() => setTab('user')}>
                    📒 Kullanıcı Defteri
                </button>
                <button className={`tab ${tab === 'stats' ? 'active' : ''}`} onClick={() => setTab('stats')}>
                    📊 İstatistikler
                </button>
            </div>

            {tab === 'user' && (
                <>
                    <UserSelect value={userId} onChange={setUserId} />

                    {loading ? (
                        <Loading />
                    ) : ledgerData ? (
                        <>
                            {/* Summary */}
                            <div className="stats-grid animate-in">
                                <div className="stat-card">
                                    <div className="stat-icon">⭐</div>
                                    <div className="stat-label">Toplam Puan</div>
                                    <div className="stat-value gold">{ledgerData.totalPoints}</div>
                                </div>
                                <div className="stat-card">
                                    <div className="stat-icon">📝</div>
                                    <div className="stat-label">İşlem Sayısı</div>
                                    <div className="stat-value">{ledgerData.transactions.length}</div>
                                </div>
                            </div>

                            {/* Chart */}
                            {chartData.length > 1 && (
                                <div className="card mb-24 animate-in stagger-1">
                                    <div className="card-header">
                                        <div className="card-title">📈 Kümülatif Puan Grafiği</div>
                                    </div>
                                    <div className="chart-container">
                                        <ResponsiveContainer width="100%" height="100%">
                                            <LineChart data={chartData}>
                                                <CartesianGrid strokeDasharray="3 3" stroke="rgba(148,163,184,0.1)" />
                                                <XAxis dataKey="name" tick={{ fill: '#94a3b8', fontSize: 10 }} />
                                                <YAxis tick={{ fill: '#94a3b8', fontSize: 12 }} />
                                                <Tooltip
                                                    contentStyle={{
                                                        background: '#1a2236',
                                                        border: '1px solid rgba(148,163,184,0.15)',
                                                        borderRadius: 8,
                                                    }}
                                                />
                                                <Line
                                                    type="monotone"
                                                    dataKey="cumulative"
                                                    stroke="#ffc107"
                                                    strokeWidth={3}
                                                    dot={{ fill: '#ffc107', r: 4 }}
                                                    name="Toplam Puan"
                                                />
                                            </LineChart>
                                        </ResponsiveContainer>
                                    </div>
                                </div>
                            )}

                            {/* Transactions */}
                            <div className="card animate-in stagger-2">
                                <div className="card-header">
                                    <div className="card-title">📒 Puan Hareketleri</div>
                                </div>
                                <div className="table-wrapper">
                                    <table className="data-table">
                                        <thead>
                                            <tr>
                                                <th>ID</th>
                                                <th>Puan</th>
                                                <th>Kaynak</th>
                                                <th>Referans</th>
                                                <th>Açıklama</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {ledgerData.transactions.map(t => (
                                                <tr key={t.ledgerId}>
                                                    <td style={{ fontFamily: 'monospace', fontSize: 12 }}>{t.ledgerId}</td>
                                                    <td>
                                                        <span className="pill pill-success" style={{ fontWeight: 700 }}>+{t.pointsDelta}</span>
                                                    </td>
                                                    <td>
                                                        <span className="pill pill-info">{t.source}</span>
                                                    </td>
                                                    <td style={{ fontFamily: 'monospace', fontSize: 12 }}>{t.sourceRef}</td>
                                                    <td style={{ color: 'var(--text-muted)', fontSize: 12 }}>{t.description || '-'}</td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </>
                    ) : null}
                </>
            )}

            {tab === 'stats' && (
                loading ? (
                    <Loading />
                ) : (
                    <div className="card animate-in">
                        <div className="card-header">
                            <div className="card-title">📊 Tüm Kullanıcı İstatistikleri</div>
                        </div>
                        <div className="table-wrapper">
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>Kullanıcı</th>
                                        <th>Toplam Puan</th>
                                        <th>İşlem Sayısı</th>
                                        <th>Ortalama</th>
                                        <th>En Yüksek</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {stats.map(s => (
                                        <tr key={s.userId}>
                                            <td>
                                                <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                                                    <div className="avatar avatar-sm">{s.userName[0]}</div>
                                                    <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{s.userName}</span>
                                                </div>
                                            </td>
                                            <td style={{ fontWeight: 800, color: 'var(--primary)' }}>{s.totalPoints}</td>
                                            <td>{s.transactionCount}</td>
                                            <td>{s.averagePointsPerTransaction}</td>
                                            <td>
                                                <span className="pill pill-success">+{s.highestSingleGain}</span>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                )
            )}
        </div>
    );
}
