import {Injectable, NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {NZ_I18N} from 'ng-zorro-antd/i18n';
import {en_US} from 'ng-zorro-antd/i18n';
import {registerLocaleData} from '@angular/common';
import en from '@angular/common/locales/en';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {IconsProviderModule} from './icons-provider.module';
import {NzLayoutModule} from 'ng-zorro-antd/layout';
import {NzMenuModule} from 'ng-zorro-antd/menu';
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzDropDownModule} from "ng-zorro-antd/dropdown";
import {HeaderComponent} from "./routes/header/header.component";
import {BestProductComponent} from "./routes/best-product/best-product.component";
import {SpecialProductComponent} from "./routes/special-product/special-product.component";
import {FooterComponent} from "./routes/footer/footer.component";
import {TestimonialsComponent} from "./routes/testimonials/testimonials.component";
import {ImageProductComponent} from "./routes/image-product/image-product.component";
import {HomeComponent} from "./routes/home/home.component";
import {DetailProductComponent} from "./routes/detail-product/detail-product.component";
import {AllProductComponent} from "./routes/all-product/all-product.component";
import {AccountComponent} from "./routes/account/account.component";
import {NzSpinModule} from "ng-zorro-antd/spin";
import {CartComponent} from "./routes/cart/cart.component";
import {NzDrawerModule} from "ng-zorro-antd/drawer";
import {NzDividerModule} from "ng-zorro-antd/divider";
import {NzCardModule} from "ng-zorro-antd/card";
import {NzAvatarModule} from "ng-zorro-antd/avatar";
import {NzSkeletonModule} from "ng-zorro-antd/skeleton";
import {NzInputModule} from "ng-zorro-antd/input";
import {NzTableModule} from "ng-zorro-antd/table";
import {NzInputNumberModule} from "ng-zorro-antd/input-number";
import {NzCommentModule} from "ng-zorro-antd/comment";
import {NzFormModule} from "ng-zorro-antd/form";
import {NzListModule} from "ng-zorro-antd/list";
import {NzBackTopModule} from "ng-zorro-antd/back-top";
import {NzCheckboxModule} from "ng-zorro-antd/checkbox";
import {NzMessageModule, NzMessageService} from "ng-zorro-antd/message";
import {JWT_OPTIONS, JwtHelperService} from "@auth0/angular-jwt";
import {PaymentsComponent} from "./routes/payments/payments.component";
import {NzModalModule} from "ng-zorro-antd/modal";
import {NotFoundComponent} from "./routes/not-found/not-found.component";
import {NzBadgeModule} from "ng-zorro-antd/badge";
import {NzRadioModule} from "ng-zorro-antd/radio";
import {NzSelectModule} from "ng-zorro-antd/select";
import {AgmCoreModule} from "@agm/core";
import {AboutUsComponent} from "./routes/about-us/about-us.component";
import {BlogComponent} from "./routes/blog/blog.component";
import {NzDatePickerModule} from "ng-zorro-antd/date-picker";

registerLocaleData(en);

@Injectable({
  providedIn: 'root',
})

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    BestProductComponent,
    SpecialProductComponent,
    FooterComponent,
    TestimonialsComponent,
    ImageProductComponent,
    HomeComponent,
    DetailProductComponent,
    AllProductComponent,
    AccountComponent,
    CartComponent,
    PaymentsComponent,
    NotFoundComponent,
    AboutUsComponent,
    BlogComponent
  ],
    imports: [
        BrowserModule,
        HttpClientModule,
        AppRoutingModule,
        FormsModule,
        BrowserAnimationsModule,
        IconsProviderModule,
        NzLayoutModule,
        NzMenuModule,
        NzButtonModule,
        NzDropDownModule,
        NzSpinModule,
        NzDrawerModule,
        NzDividerModule,
        NzCardModule,
        NzAvatarModule,
        NzSkeletonModule,
        NzInputModule,
        NzTableModule,
        NzInputNumberModule,
        NzCommentModule,
        NzFormModule,
        NzListModule,
        NzBackTopModule,
        ReactiveFormsModule,
        NzCheckboxModule,
        NzMessageModule,
        NzModalModule,
        NzBadgeModule,
        NzRadioModule,
        NzSelectModule,
        AgmCoreModule.forRoot({
            apiKey: 'AIzaSyAIso7SHcnVfjP1ygFCJaxlwO_lDQEzlAY',
            libraries: ["places", "geometry"]
        }),
        NzDatePickerModule
    ],
  providers: [
    {provide: NZ_I18N, useValue: en_US},
    JwtHelperService,
    {provide: JWT_OPTIONS, useValue: JWT_OPTIONS},
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
