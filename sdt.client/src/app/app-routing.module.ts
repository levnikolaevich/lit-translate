import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TranslatorComponent } from './components/translator/translator.component';
import { VersionComponent } from './components/version/version.component';



const appRoutes: Routes = [
  { path: '', component: TranslatorComponent },
  { path: 'version', component: VersionComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes, { useHash: false })],
  exports: [RouterModule]
})

export class AppRoutingModule {

}
