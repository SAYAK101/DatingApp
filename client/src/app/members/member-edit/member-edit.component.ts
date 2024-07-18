import {
  Component,
  HostListener,
  inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import { Member } from '../../_models/Member';
import { AccountService } from '../../_service/account.service';
import { MembersService } from '../../_service/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorComponent } from "../photo-editor/photo-editor.component";

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [TabsModule, FormsModule, PhotoEditorComponent],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    if (this.editForm?.dirty) {
      $event.retunValue = true;
    }
  }

  ngOnInit(): void {
    this.loadMember();
  }
  member?: Member;
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private toaster = inject(ToastrService);

  loadMember() {
    const user = this.accountService.currentUser();

    if (user === null) return;
    this.memberService.getMember(user.username).subscribe({
      next: (response) => (this.member = response),
    });
  }

  updateMember() {
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: (_) => {
        this.toaster.success('Profile update successfully!');
        this.editForm?.reset(this.member);
      },
    });
  }

  OnMemberChange(event : Member){
    this.member = event;
  }
}
