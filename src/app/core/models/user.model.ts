export interface User {
  id?: number;
  email: string;
  username: string;
  role: 'USER' | 'MODERATOR' | 'ADMIN';
  token?: string;
}
