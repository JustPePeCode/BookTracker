import { Link } from "react-router-dom";
import { useCurrentMember } from "../auth/useCurrentMember";

type EditAccountLinkProps = {
  memberId: number;
};

export function EditAccountLink({ memberId }: EditAccountLinkProps) {
  const currentMemberQuery = useCurrentMember();

  if (
    !currentMemberQuery.isSuccess ||
    currentMemberQuery.data.id !== memberId
  ) {
    return null;
  }

  return <Link to={`/account/edit/${memberId}`}>Edit My Account</Link>;
}
