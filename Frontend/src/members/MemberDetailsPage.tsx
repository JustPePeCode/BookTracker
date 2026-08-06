import { useQuery } from "@tanstack/react-query";
import { Link, useParams } from "react-router-dom";
import { ApiError } from "../api";
import { getMember } from "./membersApi";
import { EditMemberLink } from "./EditMemberLink";

function readMemberId(value: string | undefined) {
  const memberId = Number(value);
  return Number.isInteger(memberId) && memberId > 0 ? memberId : null;
}
export function MemberDetailsPage() {
  const { memberId: memberIdParameter } = useParams();
  const memberId = readMemberId(memberIdParameter);

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

  if (memberId === null) {
    return (
      <main>
        <h1>Invalid member id</h1>
        <Link to="/members">Back to members</Link>
      </main>
    );
  }

  if (memberQuery.isPending) {
    return <p>Loading member...</p>;
  }

  const notFound =
    memberQuery.error instanceof ApiError && memberQuery.error.status === 404;

  if (notFound) {
    return (
      <main>
        <h1>member not found</h1>
        <p>The requested member does not exist.</p>
        <Link to="/members">Back to members</Link>
      </main>
    );
  }

  if (memberQuery.isError) {
    return (
      <main>
        <h1>Could not load the member</h1>
        <p>Is the API running?</p>
        <Link to="/members">Back to members</Link>
      </main>
    );
  }

  const member = memberQuery.data;

  return (
    <main>
      <Link to="/members">Back to members</Link>
  
      <h1>Name:{member.name}</h1>
      <p>Email: {member.email}</p>
      <EditMemberLink memberId={member.id} />
    </main>
  );
}
