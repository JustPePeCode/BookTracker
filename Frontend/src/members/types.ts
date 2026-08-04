export type RegisterMemberRequest = {
  name: string;
  email: string;
  password: string;
};

export type RegisterMemberResponse = {
  id: number;
  name: string;
  email: string;
};

export type MemberDetails = {
  id: number;
  name: string;
  email: string;
};

export type MemberSummary = {
  id: number;
  name: string;
  email: string;
};

export type GetMembersRequest = {
  page: number;
  pageSize: number;
  search: string;
};

export type CreateMemberRequest = {
  id: number;
  name: string;
  email: string;
};

export type CreateMemberResponse = {
  id: number;
  name: string;
  email: string;
};

export type UpdateMemberRequest = {
  id: number;
  name: string;
  email: string;
};
