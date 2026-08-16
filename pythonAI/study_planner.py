import json
from datetime import date
from http.server import BaseHTTPRequestHandler, HTTPServer

from ML.study_model import StudyTimeModel


# Load the trained Machine Learning model once
ml_model = StudyTimeModel()


def calculate_priority(due_date):
    today = date.today()
    days_remaining = (due_date - today).days

    if days_remaining <= 0:
        return 100
    elif days_remaining <= 3:
        return 90
    elif days_remaining <= 7:
        return 75
    elif days_remaining <= 14:
        return 60
    else:
        return 40


def generate_study_plan(data):

    subjects = data.get("subjects", [])
    goals = data.get("goals", [])
    tasks = data.get("tasks", [])
    availability = data.get("availability", [])

    # ---------------------------------------------------------
    # 1. Calculate available study time
    # ---------------------------------------------------------

    total_available_hours = sum(
        float(item.get("availableHours", 0))
        for item in availability
    )

    # ---------------------------------------------------------
    # 2. Find incomplete tasks and calculate priorities
    # ---------------------------------------------------------

    incomplete_tasks = []

    for task in tasks:

        if task.get("isCompleted", False):
            continue

        try:
            due_date = date.fromisoformat(
                task["dueDate"]
            )

            priority = calculate_priority(due_date)

        except Exception:
            priority = 50

        task_copy = dict(task)
        task_copy["priorityScore"] = priority

        incomplete_tasks.append(task_copy)

    incomplete_tasks.sort(
        key=lambda task: task["priorityScore"],
        reverse=True
    )

    # ---------------------------------------------------------
    # 3. Total subject workload
    #---------------------------------------------------------

    total_subject_hours = sum(
        float(subject.get("weeklyHours", 0))
        for subject in subjects
    )

    # ---------------------------------------------------------
    # 4. Use Machine Learning to estimate study time
    # ---------------------------------------------------------

    subject_plan = []

    incomplete_count = len(incomplete_tasks)

    for subject in subjects:

        subject_name = subject.get("name", "Unknown Subject")

        weekly_hours = float(
            subject.get("weeklyHours", 0)
        )

        # Find the most urgent task
        subject_tasks = [
            task for task in incomplete_tasks
            if task.get("subjectId") is not None
        ]

        if subject_tasks:
            highest_priority = max(
                task["priorityScore"]
                for task in subject_tasks
            )
        elif incomplete_tasks:
            highest_priority = incomplete_tasks[0]["priorityScore"]
        else:
            highest_priority = 40

        # Estimate days until the nearest deadline
        nearest_days = 14

        if incomplete_tasks:

            try:
                nearest_due_date = min(
                    date.fromisoformat(task["dueDate"])
                    for task in incomplete_tasks
                )

                nearest_days = max(
                    0,
                    (nearest_due_date - date.today()).days
                )

            except Exception:
                nearest_days = 14

        # Ask the ML model for recommended study time
        recommended_hours = ml_model.predict_study_hours(
            days_until_deadline=nearest_days,
            task_priority=highest_priority,
            subject_weekly_hours=weekly_hours,
            incomplete_tasks=incomplete_count,
            available_hours=total_available_hours
        )

        subject_plan.append({
            "name": subject_name,
            "weeklyHours": weekly_hours,
            "recommendedHours": recommended_hours,
            "priorityScore": highest_priority
        })

    # ---------------------------------------------------------
    # 5. Build daily schedule
    # ---------------------------------------------------------

    daily_schedule = []

    remaining_weekly_hours = total_available_hours

    for item in availability:

        day = item.get("dayOfWeek", "Unknown")
        available_hours = float(
            item.get("availableHours", 0)
        )

        if available_hours <= 0:
            continue

        day_plan = []

        remaining_day_hours = available_hours

        for subject in subject_plan:

            if remaining_day_hours <= 0:
                break

            if remaining_weekly_hours <= 0:
                break

            recommended = float(
                subject["recommendedHours"]
            )

            if recommended <= 0:
                continue

            study_hours = min(
                recommended,
                remaining_day_hours,
                remaining_weekly_hours
            )

            if study_hours <= 0:
                continue

            day_plan.append({
                "subject": subject["name"],
                "hours": round(study_hours, 2)
            })

            remaining_day_hours -= study_hours
            remaining_weekly_hours -= study_hours

        daily_schedule.append({
            "day": day,
            "availableHours": available_hours,
            "studySessions": day_plan
        })

    # ---------------------------------------------------------
    # 6. Calculate workload coverage
    # ---------------------------------------------------------

    if total_subject_hours > 0:
        workload_coverage = (
            total_available_hours /
            total_subject_hours
        ) * 100
    else:
        workload_coverage = 0

    workload_coverage = min(
        100,
        round(workload_coverage, 1)
    )

    # ---------------------------------------------------------
    # 7. Warnings
    # ---------------------------------------------------------

    warnings = []

    if not availability:
        warnings.append(
            "No study availability has been registered. "
            "Add available study hours to generate a realistic "
            "daily schedule."
        )

    if total_available_hours <= 0:
        warnings.append(
            "Available study time is zero. "
            "The planner cannot allocate study sessions."
        )

    if total_subject_hours > total_available_hours:
        warnings.append(
            "The requested subject workload is greater than "
            "the student's available study time."
        )

    # ---------------------------------------------------------
    # 8. Planning logic
    # ---------------------------------------------------------

    planning_logic = [
        "Urgent deadlines receive higher priority.",
        "Incomplete tasks are prioritized before regular study.",
        "Machine Learning estimates recommended study hours.",
        "Subjects with higher workload receive more study attention.",
        "The planner never allocates more time than the student's registered availability."
    ]

    # ---------------------------------------------------------
    # 9. Final result
    # ---------------------------------------------------------

    return {
        "success": True,

        "summary": {
            "subjects": len(subjects),
            "goals": len(goals),
            "incompleteTasks": incomplete_count,
            "weeklyAvailableHours": total_available_hours,
            "totalSubjectHours": total_subject_hours,
            "workloadCoverage": workload_coverage
        },

        "priorityTasks": incomplete_tasks,

        "subjectPlan": subject_plan,

        "dailySchedule": daily_schedule,

        "planningLogic": planning_logic,

        "warnings": warnings
    }


class StudyPlannerHandler(BaseHTTPRequestHandler):

    def do_POST(self):

        if self.path != "/generate":

            self.send_response(404)
            self.end_headers()

            return

        try:

            content_length = int(
                self.headers.get(
                    "Content-Length",
                    0
                )
            )

            body = self.rfile.read(
                content_length
            )

            data = json.loads(
                body.decode("utf-8")
            )

            result = generate_study_plan(data)

            output = json.dumps(
                result,
                ensure_ascii=False,
                indent=2
            ).encode("utf-8")

            self.send_response(200)

            self.send_header(
                "Content-Type",
                "application/json; charset=utf-8"
            )

            self.send_header(
                "Content-Length",
                str(len(output))
            )

            self.end_headers()

            self.wfile.write(output)

        except Exception as e:

            output = json.dumps(
                {
                    "success": False,
                    "error": str(e)
                },
                ensure_ascii=False,
                indent=2
            ).encode("utf-8")

            self.send_response(500)

            self.send_header(
                "Content-Type",
                "application/json; charset=utf-8"
            )

            self.send_header(
                "Content-Length",
                str(len(output))
            )

            self.end_headers()

            self.wfile.write(output)


if __name__ == "__main__":

    server = HTTPServer(
        ("127.0.0.1", 5050),
        StudyPlannerHandler
    )

    print(
        "Python AI Study Planner running on "
        "http://127.0.0.1:5050"
    )

    print(
        "Machine Learning model loaded successfully."
    )

    print(
        "Waiting for study planning requests..."
    )

    server.serve_forever()