import {Component, Input, OnInit} from '@angular/core';
import {ChallengeRating, Monster, MonsterType, Morality, Order, Size} from "../../../../models/monster";
import {FormControl, FormGroup} from "@angular/forms";
import {MonsterService} from "../../../../services/monster.service";
import {StatsByCr, StatsByCrList} from "../../../../models/lists/statsByCrList";
import {CRList} from "../../../../models/lists/CRList";
import {zip} from "rxjs";

@Component({
  selector: 'app-monster-basic-info',
  templateUrl: './monster-basic-info.component.html',
  styleUrls: ['./monster-basic-info.component.css']
})
export class MonsterBasicInfoComponent implements OnInit {

  //input
  @Input() set monster(mon: Monster) {
    this._monster = mon;
  }
  @Input() formGroups: { [id: string]: { [id: string]: FormControl; } }

  //static
  public sizeValues = Object.values(Size);
  public monsterTypeValues = Object.values(MonsterType);
  statByCr: StatsByCr[] = StatsByCrList.list;
  tags: { [id: string]: string; }
  public crValues: ChallengeRating[] = CRList.list;

  //local
  alignment: { [id: string]: boolean; } = {};
  alignmentList: string[] = [];
  _monster: Monster;
  proficiency: number;
  hover: boolean;


  //form
  public basicFormGroup: FormGroup;
  private _group: { [id: string]: FormControl; } = {};
  _loaded = false;
  
  
  constructor(private monsterService: MonsterService) {
    let loadingTags = this.monsterService.getTags();
    loadingTags.subscribe(res => {
      this.tags = res
    });
    
    zip(loadingTags).subscribe(_ => {
          this._loaded = true;      
    });
  }
  
  static initForm(group: { [id: string]: FormControl; }) {
    group['proficiency'] = new FormControl();
    group['cr'] = new FormControl();
    group['size'] = new FormControl();
    return group;
  }

  ngOnInit(): void {
    this._group = this.formGroups['basic']
    let group = this._group;

    this.monsterUpdated();
    this.basicFormGroup = new FormGroup(group);
    
  }
  
  monsterUpdated(){
    //alignment grid
    for (let m of Object.values(Morality)) {
      for (let o of Object.values(Order)) {
        this.alignmentList.push(o + ' ' + m)
        this.alignment[o + ' ' + m] = this._monster.alignment.alignmentChances.find(a => a.alignment.morality === m && a.alignment.order === o) !== undefined;
      }
    }

    //set CR
    if (this._monster.challengeRating)
      this.setCr();

    //fix hover
    this.hover = this._monster.speed.speeds['Hover'] > 0;

    //fix dropdown values
    let fixCr = this.crValues.find(v => v.description === this._monster.challengeRating.description);
    if (fixCr !== undefined) {
      this._monster.challengeRating = fixCr;
    }
    
  }
  
  public setCr() {
    var cr = this.crFromValue(this._monster.challengeRating.value);
    var line = this.statByCr.find(l => l.cr === cr);
    if(line && this.proficiency != line.prof)
    {
      this.proficiency = line.prof
      this._group['proficiency'].setValue(this.proficiency)
      this._group['cr'].setValue(cr)
    }
  }

  public crFromValue(val: number):number{
    if (val === -3)
      return 0
    if (val === -2)
      return 1/8
    if (val === -1)
      return 1/4
    if (val === 0)
      return 1 / 2
    return val;
  }

  sizeChanged(size: number) {
    this.formGroups['basic']['size'].setValue(size)
  }

  public getTags() {
    let mtv = this.monsterTypeValues.find(mtv => this._monster.race.monsterType == mtv)
    if(mtv !== undefined)
      return this.tags[mtv];
    return "";
  }

  public hoverToggle() {
    if (this.hover) {
      this._monster.speed.speeds['Hover'] = this._monster.speed.speeds['Fly'];
      this._monster.speed.speeds['Fly'] = 0;
    }
    else {
      this._monster.speed.speeds['Fly'] = this._monster.speed.speeds['Hover'];
      this._monster.speed.speeds['Hover'] = 0;

    }
  }

}
