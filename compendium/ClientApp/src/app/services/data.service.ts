import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Monster } from "../models/monster";
import { Spell } from "../models/spell";


@Injectable()
export class DataService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  getMonsterById(id: string): Observable<Monster> {
    return this.http.get<Monster>(this.baseUrl + 'api/monsters/' + id);
  }

  getMonsters(): Observable<Monster[]> {
    return this.http.get<Monster[]>(this.baseUrl + 'api/monsters');
  }

  getSpells(): Observable<Spell[]> {
    return this.http.get<Spell[]>(this.baseUrl + 'api/spells');
  }

  getSpellById(id: string): Observable<Spell> {
    return this.http.get<Spell>(this.baseUrl + 'api/spells/' + id);
  }

  getSpellsFromIds(ids: string[]): Observable<Spell[]> {
    return this.http.post<Spell[]>(this.baseUrl + 'api/spells', ids);
  }
}
