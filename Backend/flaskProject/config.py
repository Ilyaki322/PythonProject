import os


class DevConfig:

    # JWT
    JWT_SECRET_KEY = os.getenv('SECRET_KEY', "wellwellwell322")

    # Flask db
    SQLALCHEMY_DATABASE_URI = "postgresql://admin:Aa123456@localhost:5433/pyproj"
    SQLALCHEMY_TRACK_MODIFICATIONS = False

    # Google's oauth2 settings
    # GOOGLE_CLIENT_ID = os.getenv("GOOGLE_CLIENT_ID", '480759518763-871ef8l0kf8f3bo6ocnbdk69q6brjsrc.apps'
                                                    #   '.googleusercontent.com')

    GOOGLE_CLIENT_ID = os.getenv("GOOGLE_CLIENT_ID", '480759518763-vig8uc0379tat0cg0a90orccqut7ioup.apps'
                                                     '.googleusercontent.com')

    # GOOGLE_CLIENT_SECRET = os.getenv("GOOGLE_CLIENT_SECRET", "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")

    GOOGLE_CLIENT_SECRET = os.getenv("GOOGLE_CLIENT_SECRET", "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")

    GOOGLE_REDIRECT_URI = os.getenv("GOOGLE_REDIRECT_URI", "http://localhost:5000/login/google/callback")
    FRONTEND_URI = os.getenv("FRONTEND_URI", "mygame://oauth2?token=")

