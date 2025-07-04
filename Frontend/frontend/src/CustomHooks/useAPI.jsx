import { useState, useEffect, useCallback } from 'react';

/**
 * A custom hook to fetch some api data (only get!).
 * @returns {
 *  data: the data we interested to fetch.
 *  loading: boolean, true while the fetch is in progress.
 *  error: string|null error message if the fetch fails, null otherwise.
 *  setData: function setter for the data state.
 *  refetch: function to get the data again.
 * }
 */
export default function useAPI(API, errorMsg) {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const fetchData = useCallback(async () => {
        let isMounted = true;
        try {
            setLoading(true);
            const res = await fetch(API);
            if (!res.ok) throw new Error(`${res.statusText}`);
            const json = await res.json();
            if (isMounted) setData(json);
        } catch (e) {
            if (isMounted) setError(`${errorMsg} ${e}`);
        } finally {
            if (isMounted) setLoading(false);
        }
        return () => { isMounted = false };
    }, [API, errorMsg]);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    return [data, loading, error, setData, fetchData];
}