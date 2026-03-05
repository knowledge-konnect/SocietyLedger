/**
 * Role IDs and Enums
 * Matches backend role_id values
 */

export enum RoleId {
  SOCIETY_ADMIN = 2,
  ACCOUNTANT = 3,
  MEMBER = 4,
  AUDITOR = 5,
}

export enum RoleCode {
  SOCIETY_ADMIN = 'admin',
  ACCOUNTANT = 'accountant',
  MEMBER = 'member',
  AUDITOR = 'auditor',
}

export const ROLE_ID_TO_CODE: Record<RoleId, RoleCode> = {
  [RoleId.SOCIETY_ADMIN]: RoleCode.SOCIETY_ADMIN,
  [RoleId.ACCOUNTANT]: RoleCode.ACCOUNTANT,
  [RoleId.MEMBER]: RoleCode.MEMBER,
  [RoleId.AUDITOR]: RoleCode.AUDITOR,
};

export const ROLE_CODE_TO_ID: Record<RoleCode, RoleId> = {
  [RoleCode.SOCIETY_ADMIN]: RoleId.SOCIETY_ADMIN,
  [RoleCode.ACCOUNTANT]: RoleId.ACCOUNTANT,
  [RoleCode.MEMBER]: RoleId.MEMBER,
  [RoleCode.AUDITOR]: RoleId.AUDITOR,
};

export const ROLE_LABELS: Record<RoleId | RoleCode, string> = {
  [RoleId.SOCIETY_ADMIN]: 'Society Admin',
  [RoleId.ACCOUNTANT]: 'Accountant',
  [RoleId.MEMBER]: 'Member',
  [RoleId.AUDITOR]: 'Auditor',
  [RoleCode.SOCIETY_ADMIN]: 'Society Admin',
  [RoleCode.ACCOUNTANT]: 'Accountant',
  [RoleCode.MEMBER]: 'Member',
  [RoleCode.AUDITOR]: 'Auditor',
};

export const ROLE_DESCRIPTIONS: Record<RoleId | RoleCode, string> = {
  [RoleId.SOCIETY_ADMIN]: 'Full access to all features',
  [RoleId.ACCOUNTANT]: 'Manage finances and billing',
  [RoleId.MEMBER]: 'Basic access',
  [RoleId.AUDITOR]: 'Read-only audit access',
  [RoleCode.SOCIETY_ADMIN]: 'Full access to all features',
  [RoleCode.ACCOUNTANT]: 'Manage finances and billing',
  [RoleCode.MEMBER]: 'Basic access',
  [RoleCode.AUDITOR]: 'Read-only audit access',
};
