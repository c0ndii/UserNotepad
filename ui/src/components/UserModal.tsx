import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  MenuItem,
  CircularProgress,
  IconButton,
  Typography,
  Grid,
  Stack,
} from "@mui/material";
import { Add, Delete } from "@mui/icons-material";
import { useEffect } from "react";
import { useGetUser } from "../hooks/useGetUser";
import { useAddUser } from "../hooks/useAddUser";
import { useUpdateUser } from "../hooks/useUpdateUser";
import { useForm, useFieldArray, Controller } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { AttributeTypeEnum as AttrEnum } from "../types/user";
import { useSnackbar } from "../hooks/useSnackbar";

const attributeSchema = z
  .object({
    key: z.string().optional(),
    value: z.string().optional(),
    valueType: z.enum(AttrEnum),
  })
  .refine((data) => !(data.key && !data.value), {
    message: "Value is required when key is filled",
    path: ["value"],
  });

const userSchema = z.object({
  name: z.string().min(1, "Name is required"),
  surname: z.string().min(1, "Surname is required"),
  birthDate: z
    .string()
    .min(1, "Birth date is required")
    .refine((val) => {
      const date = new Date(val);
      const today = new Date();
      date.setHours(0, 0, 0, 0);
      today.setHours(0, 0, 0, 0);
      return date <= today;
    }, "Birth date cannot be in the future"),
  sex: z.number().int(),
  attributes: z.array(attributeSchema).superRefine((attrs, ctx) => {
    const keys = attrs.map((a) => a.key).filter(Boolean);
    const duplicates = keys.filter((key, idx) => keys.indexOf(key) !== idx);
    if (duplicates.length) {
      ctx.addIssue({
        code: "custom",
        message: "Attribute keys must be unique",
        path: ["root"],
      });
    }
  }),
});

type UserForm = z.infer<typeof userSchema>;

interface UserModalProps {
  open: boolean;
  onClose: () => void;
  userId?: string;
}

export const UserModal = ({ open, onClose, userId }: UserModalProps) => {
  const isEdit = !!userId;

  const { user, isLoading: isUserLoading } = useGetUser(userId ?? "");
  const addUserMutation = useAddUser();
  const updateUserMutation = useUpdateUser(userId ?? "");
  const { showMessage } = useSnackbar();

  const {
    control,
    handleSubmit,
    reset,
    watch,
    trigger,
    formState: { errors },
  } = useForm<UserForm>({
    resolver: zodResolver(userSchema),
    defaultValues: {
      name: "",
      surname: "",
      birthDate: new Date().toISOString().split("T")[0],
      sex: 0,
      attributes: [],
    },
    mode: "all",
    reValidateMode: "onChange",
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: "attributes",
  });

  useEffect(() => {
    if (open) {
      if (isEdit && user) {
        reset({
          name: user.name,
          surname: user.surname,
          birthDate: user.birthDate.split("T")[0],
          sex: user.sex,
          attributes: user.attributes ?? [],
        });
      } else {
        reset({
          name: "",
          surname: "",
          birthDate: new Date().toISOString().split("T")[0],
          sex: 0,
          attributes: [],
        });
      }
    }
  }, [open, isEdit, user, reset]);

  const onSubmit = async (data: UserForm) => {
    try {
      const filteredAttributes =
        data.attributes?.filter((a) => a.key && a.value) ?? [];

      const formattedData = {
        ...data,
        attributes: filteredAttributes.map((a) => ({
          key: a.key!,
          value: a.value!,
          valueType: a.valueType,
        })),
      };

      if (isEdit && userId) {
        await updateUserMutation.mutateAsync(formattedData);
        showMessage("User updated successfully", "success");
      } else {
        await addUserMutation.mutateAsync(formattedData);
        showMessage("User added successfully", "success");
      }

      onClose();
    } catch (err) {
      showMessage("An error occurred", "error");
      console.error(err);
    }
  };

  const renderValueInput = (type: AttrEnum, index: number) => {
    switch (type) {
      case AttrEnum.bool:
        return (
          <Controller
            name={`attributes.${index}.value`}
            control={control}
            render={({ field }) => (
              <TextField select label="Value" fullWidth {...field}>
                <MenuItem value="true">True</MenuItem>
                <MenuItem value="false">False</MenuItem>
              </TextField>
            )}
          />
        );
      case AttrEnum.DateTime:
        return (
          <Controller
            name={`attributes.${index}.value`}
            control={control}
            render={({ field }) => (
              <TextField
                type="date"
                label="Value"
                fullWidth
                slotProps={{ inputLabel: { shrink: true } }}
                {...field}
              />
            )}
          />
        );
      case AttrEnum.double:
      case AttrEnum.int:
        return (
          <Controller
            name={`attributes.${index}.value`}
            control={control}
            render={({ field }) => (
              <TextField type="number" label="Value" fullWidth {...field} />
            )}
          />
        );
      default:
        return (
          <Controller
            name={`attributes.${index}.value`}
            control={control}
            render={({ field }) => (
              <TextField label="Value" fullWidth {...field} />
            )}
          />
        );
    }
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="md">
      <DialogTitle>{isEdit ? "Edit User" : "Add User"}</DialogTitle>
      <DialogContent dividers>
        {isUserLoading ? (
          <CircularProgress />
        ) : (
          <>
            <Controller
              name="name"
              control={control}
              render={({ field }) => (
                <TextField
                  fullWidth
                  margin="normal"
                  label="Name"
                  error={!!errors.name}
                  helperText={errors.name?.message}
                  {...field}
                />
              )}
            />

            <Controller
              name="surname"
              control={control}
              render={({ field }) => (
                <TextField
                  fullWidth
                  margin="normal"
                  label="Surname"
                  error={!!errors.surname}
                  helperText={errors.surname?.message}
                  {...field}
                />
              )}
            />

            <Controller
              name="birthDate"
              control={control}
              render={({ field }) => (
                <TextField
                  fullWidth
                  margin="normal"
                  type="date"
                  label="Birth Date"
                  error={!!errors.birthDate}
                  helperText={errors.birthDate?.message}
                  {...field}
                />
              )}
            />

            <Controller
              name="sex"
              control={control}
              render={({ field }) => (
                <TextField
                  select
                  fullWidth
                  margin="normal"
                  label="Sex"
                  {...field}
                >
                  <MenuItem value={0}>Male</MenuItem>
                  <MenuItem value={1}>Female</MenuItem>
                  <MenuItem value={2}>Other</MenuItem>
                </TextField>
              )}
            />

            <Typography variant="h6" sx={{ mt: 3 }}>
              Attributes
            </Typography>

            <Stack spacing={3} sx={{ mt: 1 }}>
              {fields.map((field, index) => (
                <Grid
                  container
                  spacing={2}
                  key={field.id}
                  alignItems="flex-start"
                >
                  <Grid>
                    <Controller
                      name={`attributes.${index}.key`}
                      control={control}
                      render={({ field }) => (
                        <TextField
                          fullWidth
                          label="Key"
                          error={!!errors.attributes?.[index]?.key}
                          {...field}
                          onChange={(e) => {
                            field.onChange(e);
                            trigger("attributes");
                          }}
                        />
                      )}
                    />
                  </Grid>

                  <Grid>
                    <Controller
                      name={`attributes.${index}.valueType`}
                      control={control}
                      render={({ field }) => (
                        <TextField select fullWidth label="Type" {...field}>
                          <MenuItem value={AttrEnum.int}>Int</MenuItem>
                          <MenuItem value={AttrEnum.double}>Double</MenuItem>
                          <MenuItem value={AttrEnum.bool}>Bool</MenuItem>
                          <MenuItem value={AttrEnum.string}>String</MenuItem>
                          <MenuItem value={AttrEnum.DateTime}>
                            DateTime
                          </MenuItem>
                        </TextField>
                      )}
                    />
                  </Grid>

                  <Grid>
                    {renderValueInput(
                      watch(`attributes.${index}.valueType`) ?? AttrEnum.string,
                      index
                    )}
                    {errors.attributes?.[index]?.value && (
                      <Typography color="error" variant="caption">
                        {errors.attributes[index]?.value?.message}
                      </Typography>
                    )}
                  </Grid>
                  {errors.attributes?.message && (
                    <Typography color="error" variant="caption">
                      {errors.attributes.message}
                    </Typography>
                  )}

                  <Grid>
                    <IconButton onClick={() => remove(index)}>
                      <Delete />
                    </IconButton>
                  </Grid>
                </Grid>
              ))}
            </Stack>

            {errors.attributes?.root?.message && (
              <Typography color="error" variant="caption" sx={{ mt: 1 }}>
                {errors.attributes.root.message}
              </Typography>
            )}
            <br />

            <Button
              sx={{ mt: 2 }}
              variant="outlined"
              startIcon={<Add />}
              onClick={() =>
                append({
                  key: "",
                  value: "",
                  valueType: AttrEnum.string,
                })
              }
            >
              Add attribute
            </Button>
          </>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color="secondary">
          Cancel
        </Button>
        <Button
          onClick={handleSubmit(onSubmit)}
          variant="contained"
          disabled={addUserMutation.isPending || updateUserMutation.isPending}
        >
          {addUserMutation.isPending || updateUserMutation.isPending ? (
            <CircularProgress size={24} color="inherit" />
          ) : isEdit ? (
            "Update"
          ) : (
            "Add"
          )}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
