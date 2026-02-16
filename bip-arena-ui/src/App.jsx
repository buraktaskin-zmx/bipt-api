import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Layout from './components/Layout';
import DashboardPage from './pages/DashboardPage';
import LeaderboardPage from './pages/LeaderboardPage';
import UsersPage from './pages/UsersPage';
import ActivitiesPage from './pages/ActivitiesPage';
import ChallengesPage from './pages/ChallengesPage';
import BadgesPage from './pages/BadgesPage';
import NotificationsPage from './pages/NotificationsPage';
import LedgerPage from './pages/LedgerPage';
import ChallengeAdminPage from './pages/ChallengeAdminPage';
import WhatIfPage from './pages/WhatIfPage';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<Layout />}>
          <Route path="/" element={<DashboardPage />} />
          <Route path="/leaderboard" element={<LeaderboardPage />} />
          <Route path="/users" element={<UsersPage />} />
          <Route path="/activities" element={<ActivitiesPage />} />
          <Route path="/challenges" element={<ChallengesPage />} />
          <Route path="/badges" element={<BadgesPage />} />
          <Route path="/notifications" element={<NotificationsPage />} />
          <Route path="/ledger" element={<LedgerPage />} />
          <Route path="/challenge-admin" element={<ChallengeAdminPage />} />
          <Route path="/whatif" element={<WhatIfPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
