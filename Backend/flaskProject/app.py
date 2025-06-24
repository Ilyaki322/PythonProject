from flask import Flask, jsonify
from db import db
from db import bcrypt
from routes.login_routes import login_route
from routes.char_selection_routes import selection_route
from flask_jwt_extended import JWTManager

app = Flask(__name__)

app.config['SQLALCHEMY_DATABASE_URI'] = 'postgresql://admin:Aa123456@localhost:5433/pyproj'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False

app.config["JWT_SECRET_KEY"] = "wellwellwell322"

jwt = JWTManager(app)


@jwt.unauthorized_loader
def unauthorized_callback(reason):
    print("❌ Unauthorized (missing token):", reason)
    return jsonify({"error": reason}), 401


@jwt.invalid_token_loader
def invalid_token_callback(reason):
    print("❌ Invalid token:", reason)
    return jsonify({"error": reason}), 422


@jwt.expired_token_loader
def expired_token_callback(jwt_header, jwt_payload):
    print("❌ Token expired")
    return jsonify({"error": "Token expired"}), 401


db.init_app(app)
bcrypt.init_app(app)

app.register_blueprint(selection_route, url_prefix='/characters')
app.register_blueprint(login_route)

if __name__ == '__main__':
    with app.app_context():
        db.create_all()
    app.run(debug=True)
