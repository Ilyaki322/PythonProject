from flask import jsonify
from werkzeug.exceptions import BadRequest

"""
register_error_handlers module

Provides a function to attach custom JSON error handlers to a Flask application.
This ensures that API endpoints consistently return JSON-formatted error responses
instead of HTML pages.
"""


def register_error_handlers(app):
    """
        Register JSON error handlers on the Flask application.

        Args:
            app (Flask): The Flask app instance where handlers will be registered.

        Returns:
            None
    """

    @app.errorhandler(404)
    def data_not_found(error):
        return jsonify({'error': 'Not Found'}), 404

    @app.errorhandler(500)
    def internal_error(error):
        return jsonify({'error': 'Internal Server Error'}), 500

    @app.errorhandler(BadRequest)
    def bad_request(error):
        return jsonify({'error': f'Bad Request: {error.description}'}), 400
