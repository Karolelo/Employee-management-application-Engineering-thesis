import { AbstractControl, ValidatorFn, ValidationErrors } from '@angular/forms';

export function futureDateValidation(control: AbstractControl): ValidationErrors | null {
  const date = new Date(control.value).setHours(0, 0, 0, 0);
  const today = new Date().setHours(0, 0, 0, 0);
  if (date < today) {
    return { futureDate: true };
  }
  return null;
}
