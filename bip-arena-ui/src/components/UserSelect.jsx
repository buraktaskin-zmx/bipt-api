import { useState, useEffect } from 'react';
import { getUsers } from '../services/api';

export default function UserSelect({ value, onChange, label = 'Kullanıcı Seç' }) {
    const [users, setUsers] = useState([]);

    useEffect(() => {
        getUsers().then(setUsers).catch(console.error);
    }, []);

    return (
        <div className="user-select-bar">
            <span className="nav-icon" style={{ fontSize: 20 }}>👤</span>
            <label style={{ fontSize: 13, fontWeight: 600, color: 'var(--text-muted)', whiteSpace: 'nowrap' }}>
                {label}:
            </label>
            <select
                className="form-select"
                style={{ maxWidth: 220 }}
                value={value}
                onChange={(e) => onChange(e.target.value)}
            >
                <option value="">-- Seçiniz --</option>
                {users.map(u => (
                    <option key={u.userId} value={u.userId}>
                        {u.name} ({u.userId}) - {u.city}
                    </option>
                ))}
            </select>
        </div>
    );
}
