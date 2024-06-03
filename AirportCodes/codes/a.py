import csv

csv_file = 'airports.csv'
xml_file = 'airports.xml'

with open(csv_file, 'r') as csvfile, open(xml_file, 'w') as xmlfile:
    reader = csv.DictReader(csvfile, fieldnames=['IATA', 'ICAO', 'Airport'])
    head_row = next(reader)  # next()方法用于移动指针
    xmlfile.write('<DataTable>\n')
    for row in reader:
        iata = row['IATA']
        icao = row['ICAO']
        name = row['Airport']
        xmlfile.write(f'<CityCode FourCode="{icao}" AirportName="{name}" ThreeCode="{iata}" />\n')
    xmlfile.write('</DataTable>')
