FROM python:3.9.6-slim-buster

WORKDIR /src

COPY requirements.txt requirements.txt

RUN pip3 install -r requirements.txt

COPY ./src .

CMD ["python3", "server.py"]