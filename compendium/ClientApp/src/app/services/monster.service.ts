import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {Monster, Trait} from "../models/monster";


@Injectable()
export class MonsterService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    getMonsterById(id: string): Observable<Monster> {
        return this.http.get<Monster>(this.baseUrl + 'api/monster/' + id);
    }

    getMonsters(): Observable<Monster[]> {
        return this.http.get<Monster[]>(this.baseUrl + 'api/monster');
    }

    getMonstersFromIds(ids: string[]): Observable<Monster[]> {
        return this.http.post<Monster[]>(this.baseUrl + 'api/monster/fromIds', ids);
    }

    saveMonster(monster: Monster): Observable<Monster> {
        return this.http.post<Monster>(this.baseUrl + 'api/monster', monster);
    }

    getTags(): Observable<{ [id: string]: string; }> {
        return this.http.get<{ [id: string]: string; }>(this.baseUrl + 'api/monster/tags');
    }

    getTraits() {
        return this.http.get<{ [id: string]: Trait[]; }>(this.baseUrl + 'api/monster/traits');
    }
}
