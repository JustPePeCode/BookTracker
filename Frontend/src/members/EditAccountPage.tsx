import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate, useParams } from "react-router-dom";
import { ApiError } from "../api";
import { getMember, updateMember } from "./MemberApi";
import type { UpdateMemberRequest } from "./types";
import { removeAccessToken } from "../auth/tokenStorage";
import { DeleteMemberButton } from "./DeleteMemberButton";

function readMemberId(value: string | undefined) {
  const memberId = Number(value);
  return Number.isInteger(memberId) && memberId > 0 ? memberId : null;
}

export function EditAccountPage() {
  const { memberId: memberIdParameter } = useParams();
  const memberId = readMemberId(memberIdParameter);
  const [formError, setFormError] = useState<string | null>(null);
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const memberQuery = useQuery({
    queryKey: ["members", "detail", memberId],
    queryFn: () => {
      if (memberId === null) {
        throw new Error("Invalid member id");
      }

      return getMember(memberId);
    },
    enabled: memberId !== null,
    retry: false,
  });

  const updateMutation = useMutation({
    mutationFn: (request: UpdateMemberRequest) => {
      if (memberId === null) {
        throw new Error("Invalid member id");
      }

      return updateMember(memberId, request);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["members"],
        refetchType: "none",
      });
      removeAccessToken();
      queryClient.removeQueries({ queryKey: ["current-member"] });
      navigate("/login");
    },
  });
  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    if (!memberQuery.data) {
      return;
    }

    const formData = new FormData(event.currentTarget);
    const name = formData.get("name")?.toString().trim() ?? "";
    const email = formData.get("email")?.toString().trim() ?? "";

    if (!name || !email) {
      setFormError("Enter a valid name and email.");
      return;
    }
    updateMutation.mutate({
      name,
      email,
    });
  }

  async function reloadLatest() {
    updateMutation.reset();
    await memberQuery.refetch();
  }
  if (memberId === null) {
    return (
      <main>
        <h1>Invalid Account id</h1>
        <Link to="/account">Back to Account</Link>
      </main>
    );
  }

  if (memberQuery.isPending) {
    return <p>Loading Account...</p>;
  }

  const queryNotFound =
    memberQuery.error instanceof ApiError && memberQuery.error.status === 404;

  if (queryNotFound) {
    return (
      <main>
        <h1>Account not found</h1>
        <Link to="/account">Back to your account</Link>
      </main>
    );
  }

  if (memberQuery.isError) {
    return <p>Could not load your account.</p>;
  }

  const member = memberQuery.data;
  const mutationStatus =
    updateMutation.error instanceof ApiError
      ? updateMutation.error.status
      : null;

  const mutationErrorMessage =
    updateMutation.error instanceof ApiError
      ? updateMutation.error.message
      : null;
  return (
    <main>
      <Link to="/account">Cancel</Link>
      <h1>Edit {member.name}</h1>

      <form onSubmit={handleSubmit}>
        <label>
          {" "}
          Name:{" "}
          <input
            name="name"
            defaultValue={member.name}
            maxLength={100}
            required
          />
        </label>
        <label>
          {" "}
          Email:{" "}
          <input
            name="email"
            defaultValue={member.email}
            maxLength={100}
            required
          />
        </label>
        <button type="submit" disabled={updateMutation.isPending}>
          {updateMutation.isPending ? "Saving..." : "Save changes"}
        </button>{" "}
        TEST
        <DeleteMemberButton memberId={member.id} name={member.name} />
      </form>

      {formError && <p>{formError}</p>}
      {mutationStatus === 400 && <p>The API rejected the member data.</p>}
      {mutationStatus === 401 && <p>Your login is missing or expired.</p>}
      {mutationStatus === 403 && <p>Only administrators can edit members.</p>}
      {mutationStatus === 404 && <p>This member no longer exists.</p>}
      {mutationStatus === 409 && (
        <div>
          <p>{mutationErrorMessage}</p>
          <button type="button" onClick={reloadLatest}>
            Load latest version
          </button>
        </div>
      )}
      {updateMutation.isError && mutationStatus === null && (
        <p>Could not update your Account.</p>
      )}
    </main>
  );
}
