import { NavLink, Outlet, useLocation } from 'react-router-dom';

const navItems = [
    { section: 'Genel' },
    { path: '/', label: 'Dashboard', icon: '🏠' },
    { path: '/leaderboard', label: 'Leaderboard', icon: '🏆' },
    { section: 'Kullanıcılar' },
    { path: '/users', label: 'Kullanıcılar', icon: '👥' },
    { path: '/activities', label: 'Aktiviteler', icon: '📊' },
    { section: 'Sistem' },
    { path: '/challenges', label: 'Challenge\'lar', icon: '🎯' },
    { path: '/badges', label: 'Rozetler', icon: '🏅' },
    { path: '/notifications', label: 'Bildirimler', icon: '🔔' },
    { path: '/ledger', label: 'Puan Defteri', icon: '📒' }

];

const pageTitles = {
    '/': 'Dashboard',
    '/leaderboard': 'Leaderboard',
    '/users': 'Kullanıcılar',
    '/activities': 'Aktiviteler',
    '/challenges': 'Challenge Motoru',
    '/badges': 'Rozet Sistemi',
    '/notifications': 'Bildirimler',
    '/ledger': 'Puan Defteri',
    '/challenge-admin': 'Challenge Yönetimi',
    '/whatif': 'What-If Simülasyon',
};

export default function Layout() {
    const location = useLocation();
    const pageTitle = pageTitles[location.pathname] || 'BiP Arena';

    return (
        <div className="app-layout">
            <aside className="sidebar">
                <div className="sidebar-brand">
                    <div className="sidebar-brand-icon">
                        <img src="/Bip_logo.png" alt="BiP" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                    </div>
                    <div>
                        <h1>BiP Arena</h1>
                        <span>Social League</span>
                    </div>
                </div>
                <nav className="sidebar-nav">
                    {navItems.map((item, i) =>
                        item.section ? (
                            <div key={i} className="sidebar-section-title">{item.section}</div>
                        ) : (
                            <NavLink
                                key={item.path}
                                to={item.path}
                                end={item.path === '/'}
                                className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}
                            >
                                <span className="nav-icon">{item.icon}</span>
                                {item.label}
                            </NavLink>
                        )
                    )}
                </nav>
            </aside>

            <main className="main-content">
                <header className="topbar">
                    <div className="topbar-left">
                        <h2 className="topbar-title">{pageTitle}</h2>
                    </div>
                    <div className="topbar-right">
                        <span style={{ fontSize: 12, color: 'var(--text-muted)' }}>
                            Turkcell BiP Social Arena League
                        </span>
                    </div>
                </header>
                <div className="page-container">
                    <Outlet />
                </div>
            </main>
        </div>
    );
}
