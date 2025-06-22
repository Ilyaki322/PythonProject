from flask import Flask
from db import db
from db import bcrypt
from routes.login_routes import login_route

from models.user import User

app = Flask(__name__)

app.config['SQLALCHEMY_DATABASE_URI'] = 'postgresql://admin:Aa123456@localhost:5433/pyproj'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False

db.init_app(app)
bcrypt.init_app(app)

app.register_blueprint(login_route)

if __name__ == '__main__':
    with app.app_context():
        db.create_all()
    app.run(debug=True)
