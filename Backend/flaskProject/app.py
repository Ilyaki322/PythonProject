from flask import Flask, jsonify
from db import db
from db import bcrypt
from routes.login_routes import login_route
from routes.char_selection_routes import selection_route
from routes.inventory_routes import inventory_route
from flask_jwt_extended import JWTManager

app = Flask(__name__)

app.config['SQLALCHEMY_DATABASE_URI'] = 'postgresql://admin:Aa123456@localhost:5433/pyproj'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False

app.config["JWT_SECRET_KEY"] = "wellwellwell322"

jwt = JWTManager(app)

db.init_app(app)
bcrypt.init_app(app)

app.register_blueprint(selection_route, url_prefix='/characters')
app.register_blueprint(login_route)
app.register_blueprint(inventory_route, url_prefix='/inventory')

if __name__ == '__main__':
    with app.app_context():
        db.create_all()
    app.run(debug=True)
