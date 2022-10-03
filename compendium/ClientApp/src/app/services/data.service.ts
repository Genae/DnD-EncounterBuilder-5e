import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Monster } from "../models/monster";
import { Spell } from "../models/spell";
import { WeaponType } from '../models/weapon';


@Injectable()
export class DataService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    getMonsterById(id: string): Observable<Monster> {
        return this.http.get<Monster>(this.baseUrl + 'api/monsters/' + id);
    }

    getMonsters(): Observable<Monster[]> {
        return this.http.get<Monster[]>(this.baseUrl + 'api/monsters');
    }

    getMonstersFromIds(ids: string[]): Observable<Monster[]> {
        return this.http.post<Monster[]>(this.baseUrl + 'api/monsters', ids);
    }

    saveMonster(monster: Monster): Observable<Monster> {
        return this.http.post<Monster>(this.baseUrl + 'api/monster', monster);
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

    getTags(): Observable<{ [id: string]: string; }> {
        return this.http.get<{ [id: string]: string; }>(this.baseUrl + 'api/tags');
    }

    getWeapons(): Observable<WeaponType[]> {
        return this.http.get<WeaponType[]>(this.baseUrl + 'api/weapons');
    }
}
