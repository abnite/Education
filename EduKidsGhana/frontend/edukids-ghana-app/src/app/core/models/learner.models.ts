export interface LearnerProfile {
  id: string;
  userId: string;
  displayName: string;
  age: number;
  gradeLevel: number;
  avatarCode?: string;
  totalPoints: number;
  currentStreak: number;
  longestStreak: number;
  audioEnabled: boolean;
  preferredLanguage: string;
  lastStudyDate?: string;
}

export interface LearnerDashboard {
  profile: LearnerProfile;
  todayMinutes: number;
  dailyGoalMinutes: number;
  currentStreak: number;
  subjectProgress: SubjectProgress[];
  recentActivity: RecentActivity[];
  dailyChallenge?: DailyChallenge;
  newBadges: NewBadge[];
}

export interface SubjectProgress {
  subjectId: string;
  subjectName: string;
  colourHex: string;
  iconUrl?: string;
  completedTopics: number;
  totalTopics: number;
  progressPercent: number;
}

export interface RecentActivity {
  activityType: string;
  title: string;
  pointsEarned: number;
  activityAt: string;
}

export interface DailyChallenge {
  challengeId: string;
  title: string;
  description: string;
  rewardPoints: number;
}

export interface NewBadge {
  badgeCode: string;
  name: string;
  iconUrl?: string;
}

export interface Avatar {
  code: string;
  name: string;
  imageUrl: string;
}

export interface CreateLearnerRequest {
  displayName: string;
  age: number;
  gradeLevel: number;
  avatarCode?: string;
  audioEnabled: boolean;
}
