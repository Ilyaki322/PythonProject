import eventlet
from flask import Flask
from flask_socketio import SocketIO
from db import db
from db import bcrypt
from routes.login_routes import login_route
from routes.char_selection_routes import selection_route
from routes.inventory_routes import inventory_route
from routes import socket_routes
from flask_jwt_extended import JWTManager
from config import DevConfig
from routes.oauth_routes import oauth_bp
from flask_cors import CORS
from errors.errors import register_error_handlers
import routes.socket_routes

eventlet.monkey_patch()

app = Flask(__name__)
CORS(app, origins=["http://localhost:3000"])
app.config['PROPAGATE_EXCEPTIONS'] = True
app.config.from_object(DevConfig)

socketio = SocketIO(app, cors_allowed_origins="*", async_mode='eventlet', logger=True, engineio_logger=True)

jwt = JWTManager(app)

db.init_app(app)
bcrypt.init_app(app)

app.register_blueprint(login_route)
app.register_blueprint(oauth_bp, url_prefix='/login')
app.register_blueprint(selection_route, url_prefix='/characters')
app.register_blueprint(inventory_route, url_prefix='/inventory')

register_error_handlers(app)
routes.socket_routes.init_socket_handlers(app, socketio)

if __name__ == '__main__':
    with app.app_context():
        db.create_all()

    socketio.run(app, host='0.0.0.0', port=5000, debug=True)
