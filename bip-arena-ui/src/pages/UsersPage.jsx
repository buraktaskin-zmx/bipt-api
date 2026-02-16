import { useState, useEffect } from 'react';
import { getUsers } from '../services/api';
import Loading from '../components/Loading';

export default function UsersPage() {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getUsers()
            .then(setUsers)
            .catch(console.error)
            .finally(() => setLoading(false));
    }, []);

    if (loading) return <Loading />;

    return (
        <div>
            <div className="card animate-in">
                <div className="card-header">
                    <div className="card-title">👥 Tüm Kullanıcılar</div>
                    <span className="pill pill-gold">{users.length} kullanıcı</span>
                </div>

                <div className="table-wrapper">
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Ad</th>
                                <th>Şehir</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map(u => (
                                <tr key={u.userId}>
                                    <td>
                                        <span style={{ fontFamily: 'monospace', background: 'var(--bg-base)', padding: '2px 8px', borderRadius: 4, fontSize: 12 }}>
                                            {u.userId}
                                        </span>
                                    </td>
                                    <td>
                                        <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                                            <div className="avatar avatar-sm">{u.name[0]}</div>
                                            <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{u.name}</span>
                                        </div>
                                    </td>
                                    <td>📍 {u.city}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}
