import {Component, Input, OnInit} from '@angular/core';
import {Monster} from "../../../../../models/monster";
import {FormControl} from "@angular/forms";

@Component({
  selector: 'app-spellcasting-info',
  templateUrl: './spellcasting-info.component.html',
  styleUrls: ['./spellcasting-info.component.css']
})
export class SpellcastingInfoComponent implements OnInit {
  //Inputs
  @Input() set monster(m: Monster) {
    this._monster = m;
  }
  @Input() formGroups: { [id: string]: { [id: string]: FormControl; } }
  
  //Local
  _monster:Monster;
  constructor() { }

  ngOnInit(): void {
  }

}
