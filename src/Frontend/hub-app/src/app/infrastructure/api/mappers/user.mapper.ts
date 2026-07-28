import { BffSessionUser } from '@vyatka-tracker/auth';

import { User } from '../../../application/models/user.model';

export function mapBffSessionUserToUser(dto: BffSessionUser): User {
  return {
    id: dto.id,
    nickname: dto.nickname,
  };
}
