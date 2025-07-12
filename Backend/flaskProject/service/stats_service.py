from flask import send_file, jsonify
import pandas as pd
from models.match import Match
from sqlalchemy import or_, func
from io import BytesIO
from db import db


def export_user_matches_to_excel(user_id: int):
    matches = Match.query.filter(
        or_(
            Match.player_1_id == user_id,
            Match.player_2_id == user_id
        )
    ).all()

    if not matches:
        return jsonify({'message': 'No matches found for user.'}), 404

    # Convert matches to a list of dicts
    match_data = [{
        'Match ID': match.id,
        'Date': match.match_start,
        'Player 1 ID': match.player_1_id,
        'Player 2 ID': match.player_2_id,
        'Winner ID': match.winner_id
    } for match in matches]

    df = pd.DataFrame(match_data)

    # Stats
    total_matches = len(df)
    wins = db.session.query(func.count(Match.id)).filter(
        Match.winner_id == user_id
    ).scalar()
    winrate = (wins / total_matches) * 100
    last_match_time = df['Date'].max()

    # Add stats at the bottom of the Excel file
    stats = {
        'Total Matches': [total_matches],
        'Wins': [wins],
        'Win Rate (%)': [round(winrate, 2)],
        'Last Match Time': [last_match_time]
    }
    stats_df = pd.DataFrame(stats)

    output = BytesIO()
    with pd.ExcelWriter(output, engine='openpyxl') as writer:
        df.to_excel(writer, sheet_name='Matches', index=False)
        stats_df.to_excel(writer, sheet_name='Summary', index=False)

    output.seek(0)

    filename = f"user_{user_id}_matches.xlsx"
    return send_file(
        output,
        mimetype='application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        as_attachment=True,
        download_name=filename
    )
