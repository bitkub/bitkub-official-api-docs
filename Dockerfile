FROM python

COPY . .
RUN pip install python-dotenv
RUN pip install requests

CMD ["python", "./samples/python/sample_balances.py"]