from flask import Flask, jsonify
from db import db
from db import bcrypt
from routes.login_routes import login_route
from routes.char_selection_routes import selection_route
from routes.inventory_routes import inventory_route
from flask_jwt_extended import JWTManager
from config import DevConfig
from routes.oauth_routes import oauth_bp
from flask_cors import CORS

app = Flask(__name__)
CORS(app, origins=["http://localhost:3000"])
app.config.from_object(DevConfig)

jwt = JWTManager(app)

db.init_app(app)
bcrypt.init_app(app)

app.register_blueprint(selection_route, url_prefix='/characters')
app.register_blueprint(login_route)
app.register_blueprint(oauth_bp, url_prefix='/login')
app.register_blueprint(inventory_route, url_prefix='/inventory')

if __name__ == '__main__':
    with app.app_context():
        db.create_all()
    app.run(debug=True)
