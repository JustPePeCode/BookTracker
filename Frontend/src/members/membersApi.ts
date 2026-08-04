import { apiRequest } from "../api";
import type { PagedResult } from "../types";
import type {
  RegisterMemberRequest,
  RegisterMemberResponse,
  MemberDetails,
  GetMembersRequest,
  MemberSummary,
} from "./types";

export function registerMember(request: RegisterMemberRequest) {
  return apiRequest<RegisterMemberResponse>("/members", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function getMember(memberId: number) {
  return apiRequest<MemberDetails>(`/members/${memberId}`);
}

export function getMembers(request: GetMembersRequest) {
  const parameters = new URLSearchParams({
    page: request.page.toString(),
    pageSize: request.pageSize.toString(),
  });

  if (request.search) {
    parameters.set("search", request.search);
  }

  return apiRequest<PagedResult<MemberSummary>>(
    `/members?${parameters.toString()}`,
  );
}
