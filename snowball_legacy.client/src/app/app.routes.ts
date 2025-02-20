import { Routes } from "@angular/router";
import { AppLayout } from "./layout/component/app.layout";
import { Dashboard } from "./pages/dashboard/dashboard";

export const appRoutes: Routes = [{
  path: '',
  component: AppLayout,
  children: [
    { path: '', component: Dashboard }
  ]
}];
