import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'courses',
  templateUrl: './courses.component.html'
})
export class CoursesComponent {
  public courses: Course[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Course[]>(baseUrl + 'courses').subscribe(result => {
      this.courses = result;
    }, error => console.error(error));
  }
}

interface Course {
  name: string;
  description: string;
  price: number;
  duration: string;
}
