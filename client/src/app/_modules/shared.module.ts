import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }), 
    NgxGalleryModule,
    NgxSpinnerModule.forRoot({
      type: 'ball-atom'
    }),
    FileUploadModule
  ],
  exports:[
    BsDropdownModule,
    ToastrModule,
    NgxGalleryModule,
    NgxSpinnerModule,
    FileUploadModule
  ]
})
export class SharedModule { }
