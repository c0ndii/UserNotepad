import { SexEnum } from "../types/user";

export const formatSex = (sex: SexEnum | undefined | null): string => {
  switch (sex) {
    case SexEnum.Male:
      return "Male";
    case SexEnum.Female:
      return "Female";
    case SexEnum.Other:
      return "Other";
    default:
      return "-";
  }
};
