import joblib
import numpy as np

from sklearn.ensemble import RandomForestRegressor


# Training data
#
# Features:
# 1. days_until_deadline
# 2. task_priority
# 3. subject_weekly_hours
# 4. incomplete_tasks
# 5. available_hours
#
# Target:
# recommended study hours


X = np.array([
    [1, 100, 8, 5, 2],
    [2, 90, 6, 4, 3],
    [3, 90, 8, 3, 5],
    [5, 75, 6, 2, 6],
    [7, 75, 5, 2, 8],
    [10, 60, 4, 1, 8],
    [14, 60, 3, 1, 10],
    [21, 40, 3, 0, 10],
    [30, 40, 2, 0, 12],
    [2, 100, 10, 6, 4],
    [4, 90, 9, 4, 5],
    [6, 75, 7, 3, 7],
    [12, 60, 5, 2, 8],
    [20, 40, 4, 0, 10],
])

y = np.array([
    2.0,
    2.5,
    3.0,
    2.5,
    2.0,
    1.5,
    1.5,
    1.0,
    0.5,
    3.0,
    3.0,
    2.5,
    2.0,
    1.0,
])


# Create ML model
model = RandomForestRegressor(
    n_estimators=100,
    random_state=42
)


# Train
model.fit(X, y)


# Save trained model
joblib.dump(
    model,
    "study_time_model.joblib"
)


print("Machine Learning model trained successfully.")
print("Model saved as study_time_model.joblib")