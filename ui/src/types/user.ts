export enum SexEnum {
  Male = 0,
  Female = 1,
  Other = 2,
}

export enum AttributeTypeEnum {
  int = 0,
  double = 1,
  bool = 2,
  string = 3,
  Date = 4,
}

export interface UserAttributeDto {
  key: string;
  value: string;
  valueType: AttributeTypeEnum;
}

export interface UserDto {
  id: string;
  name: string;
  surname: string;
  birthDate: string;
  sex: SexEnum;
  attributes?: UserAttributeDto[];
}

export interface PageDto<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface UserInput {
  name: string;
  surname: string;
  birthDate: string;
  sex: SexEnum;
  attributes?: UserAttributeDto[];
}
