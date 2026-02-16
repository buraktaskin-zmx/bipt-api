const API_BASE = 'http://localhost:5254/api';

async function request(url, options = {}) {
  const res = await fetch(`${API_BASE}${url}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });
  if (!res.ok) throw new Error(`API Error: ${res.status}`);
  return res.json();
}

// Users
export const getUsers = () => request('/users');
export const getUser = (id) => request(`/users/${id}`);

// Groups
export const getGroups = () => request('/groups');

// Activities
export const getUserActivities = (userId) => request(`/activities/${userId}`);
export const getActivitiesSummary = () => request('/activities/summary');

// Metrics
export const getUserMetrics = (userId, asOfDate) =>
  request(`/metrics/${userId}?asOfDate=${asOfDate}`);
export const calculateMetrics = (asOfDate) =>
  request(`/metrics/calculate?asOfDate=${asOfDate}`, { method: 'POST' });

// Challenges
export const getChallenges = () => request('/challenges');
export const evaluateChallenges = (asOfDate) =>
  request(`/challenges/evaluate?asOfDate=${asOfDate}`, { method: 'POST' });
export const getUserChallengeAwards = (userId, asOfDate) =>
  request(`/challenges/awards/${userId}?asOfDate=${asOfDate}`);

// Leaderboard
export const getLeaderboard = () => request('/leaderboard');

// Ledger
export const getUserLedger = (userId) => request(`/ledger/${userId}`);
export const getUserTotalPoints = (userId) => request(`/ledger/${userId}/total`);
export const getLedgerStatistics = () => request('/ledger/statistics');
export const getPointsBreakdown = (userId) => request(`/ledger/${userId}/breakdown`);

// Badges
export const getAllBadges = () => request('/badges');
export const getUserBadges = (userId) => request(`/badges/user/${userId}`);
export const evaluateBadges = () => request('/badges/evaluate', { method: 'POST' });
export const evaluateUserBadges = (userId) =>
  request(`/badges/evaluate/${userId}`, { method: 'POST' });

// Notifications
export const getUserNotifications = (userId) => request(`/notifications/${userId}`);
export const getUnreadNotifications = (userId) => request(`/notifications/${userId}/unread`);
export const markNotificationRead = (notificationId) =>
  request(`/notifications/${notificationId}/mark-read`, { method: 'PUT' });
export const markAllNotificationsRead = (userId) =>
  request(`/notifications/${userId}/mark-all-read`, { method: 'PUT' });

// Dashboard
export const getUserDashboard = (userId) => request(`/dashboard/${userId}`);
