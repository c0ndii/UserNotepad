import { useQuery } from "@tanstack/react-query";
import { api } from "../api/api";
import { useSearchParams } from "react-router-dom";
import type { PageDto, UserDto } from "../types/user";

const PAGE_SIZE = 10;

export const useGetUsers = () => {
  const [searchParams, setSearchParams] = useSearchParams();

  const page = Number(searchParams.get("page") || 1);
  const pageSize = PAGE_SIZE;

  const { data, isLoading, isError, error } = useQuery<PageDto<UserDto>>({
    queryKey: ["users", page],
    queryFn: async () => {
      const res = await api.get<PageDto<UserDto>>("/users", {
        params: { page, pageSize },
      });
      return res.data;
    },
    placeholderData: (prev) => prev,
  });

  const setPage = (newPage: number) => {
    setSearchParams({ page: newPage.toString() });
  };

  return {
    users: data?.items ?? [],
    totalCount: data?.totalCount ?? 0,
    page,
    pageSize,
    setPage,
    isLoading,
    isError,
    error,
  };
};
