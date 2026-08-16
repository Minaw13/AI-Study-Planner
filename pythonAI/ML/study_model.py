import os
import joblib


MODEL_PATH = os.path.join(
    os.path.dirname(__file__),
    "study_time_model.joblib"
)


class StudyTimeModel:

    def __init__(self):
        if not os.path.exists(MODEL_PATH):
            raise FileNotFoundError(
                "Machine Learning model not found."
            )

        self.model = joblib.load(MODEL_PATH)

    def predict_study_hours(
        self,
        days_until_deadline,
        task_priority,
        subject_weekly_hours,
        incomplete_tasks,
        available_hours
    ):
        features = [[
            float(days_until_deadline),
            float(task_priority),
            float(subject_weekly_hours),
            float(incomplete_tasks),
            float(available_hours)
        ]]

        prediction = self.model.predict(features)[0]

        # Never recommend negative study time
        prediction = max(0.0, float(prediction))

        # Never exceed the student's available daily time
        prediction = min(
            prediction,
            float(available_hours)
        )

        return round(prediction, 2)


if __name__ == "__main__":

    model = StudyTimeModel()

    result = model.predict_study_hours(
        days_until_deadline=2,
        task_priority=90,
        subject_weekly_hours=6,
        incomplete_tasks=2,
        available_hours=3
    )

    print(
        f"Recommended study hours: {result}"
    )