import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FamilyNode } from '../models/family-node.model';
import { UpdateNodeArgs } from '../models/update-node-args.model';

@Injectable({
  providedIn: 'root'
})
export class FamilyService {

  constructor(private http: HttpClient) { }

  getFamilyNodes() {
    return this.http.get<FamilyNode[]>("/family/getfamilytreenodes");
  }

  getForecasts() {
    return this.http.get<WeatherForecast[]>('/weatherforecast');
  }

  updateFamilyTreeNodes(args: UpdateNodeArgs) {
    return this.http.post<{ [key: string]: string | number}>("/family/updatefamilytreenodes", args);
  }

}

export interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
