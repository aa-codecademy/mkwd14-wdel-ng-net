import { inject, Service } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

const SNACK_BAR_CONFIG: MatSnackBarConfig = {
    duration: 4000,
    verticalPosition: 'top',
    horizontalPosition: 'end',
};

@Service()
export class NotificationService {
    private readonly snackBar = inject(MatSnackBar);

    success(message: string): void {
        this.snackBar.open(message, 'Close', SNACK_BAR_CONFIG);
    }

    error(message: string): void {
        this.snackBar.open(message, 'Close', { ...SNACK_BAR_CONFIG, panelClass: 'snackbar-error' });
    }
}
