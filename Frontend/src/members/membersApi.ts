import { apiRequest } from "../api";
import type { PagedResult } from "../types";
import type {
  GetMembersRequest,
  RegisterMemberRequest,
  RegisterMemberResponse,
  MemberSummary,
} from "./types";

export function registerMember(request: RegisterMemberRequest) {
  return apiRequest<RegisterMemberResponse>("/members", {
    method: "POST",
    body: JSON.stringify(request),
  });
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
