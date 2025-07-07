from flask import jsonify
from werkzeug.exceptions import BadRequest


def register_error_handlers(app):
    @app.errorhandler(404)
    def data_not_found(error):
        return jsonify({'error': 'Not Found'}), 404

    @app.errorhandler(500)
    def internal_error(error):
        return jsonify({'error': 'Internal Server Error'}), 500

    @app.errorhandler(BadRequest)
    def bad_request(error):
        return jsonify({'error': f'Bad Request: {error.description}'}), 400
